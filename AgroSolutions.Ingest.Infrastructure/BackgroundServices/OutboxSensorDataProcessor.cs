using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Events;
using AgroSolutions.Ingest.Domain.Messaging;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
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

            List<OutboxSensorData> pendingOutboxSensorDatas = unitOfWork.OutboxReceivedSensorDatas.GetPendingOutboxSensorDataTracking();

            foreach (OutboxSensorData outboxSensorData in pendingOutboxSensorDatas)
            {
                try
                {
                    ReceivedSensorDataEvent? receivedSensorDataEvent = JsonSerializer.Deserialize<ReceivedSensorDataEvent>(outboxSensorData.Payload);
                    if (receivedSensorDataEvent is null)
                    {
                        Log.Error("Invalid ReceivedSensorDataEvent payload: {Payload}", outboxSensorData.Payload);
                        outboxSensorData.MarkAsFailed();
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        return;
                    }

                    await eventPublisher.PublishAsync(receivedSensorDataEvent, cancellationToken, receivedSensorDataEvent.CorrelationId);
                    outboxSensorData.MarkAsProcessed();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error during publish message from databse to messagin system.");
                    outboxSensorData.MarkAsFailed();
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await Task.Delay(5000, cancellationToken);
        }
    }
}
