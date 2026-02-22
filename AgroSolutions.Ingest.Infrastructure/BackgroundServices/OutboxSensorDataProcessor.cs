using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Events;
using AgroSolutions.Ingest.Domain.Messaging;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using System.Text.Json;

namespace AgroSolutions.Ingest.Infrastructure.BackgroundServices;

public class OutboxSensorDataProcessor(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            IEventPublisher eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            List<OutboxSensorData> pendingOutboxSensorDatas = unitOfWork.OutboxSensorDatas.GetPendingOutboxSensorDataTracking();
            Log.Information("Total pending of processing in database {Total}.", pendingOutboxSensorDatas.Count);

            foreach (OutboxSensorData outboxSensorData in pendingOutboxSensorDatas)
            {
                using (LogContext.PushProperty("CorrelationId", outboxSensorData.Correlationid.ToString()))
                {
                    Log.Information("Starting the processor data from sensor Id {SensorClientId}.", outboxSensorData.OutboxSensorDataId);

                    try
                    {
                        ReceivedSensorDataEvent? receivedSensorDataEvent = JsonSerializer.Deserialize<ReceivedSensorDataEvent>(outboxSensorData.Payload);
                        if (receivedSensorDataEvent is null)
                        {
                            Log.Error("Invalid ReceivedSensorDataEvent payload: {Payload}.", outboxSensorData.Payload);
                            outboxSensorData.MarkAsFailed();
                            await unitOfWork.SaveChangesAsync(cancellationToken);
                            return;
                        }

                        await eventPublisher.PublishAsync(receivedSensorDataEvent, cancellationToken, outboxSensorData.Correlationid.ToString());
                        outboxSensorData.MarkAsProcessed();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error during publish message from databse to messaging system.");
                        outboxSensorData.MarkAsFailed();
                    }
                }

                Log.Information("Finished the processor data from sensor Id {SensorClientId}.", outboxSensorData.OutboxSensorDataId);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await Task.Delay(5000, cancellationToken);
        }
    }
}
