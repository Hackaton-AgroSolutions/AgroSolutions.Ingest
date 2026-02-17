using AgroSolutions.Ingest.Domain.Events;
using AgroSolutions.Ingest.Domain.Messaging;
using AgroSolutions.Ingest.Infrastructure.Extensions;
using AgroSolutions.Ingest.Infrastructure.Messaging;
using AgroSolutions.Ingest.Infrastructure.Services;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;
using System.Text;
using System.Text.Json;

namespace AgroSolutions.Ingest.Infrastructure.Subscribers;

public class ReceivedSensorDataSubscriber(IServiceProvider serviceProvider, IOptions<RabbitMqOptions> options) : BackgroundService
{
    private readonly string _queue = options.Value.Destinations.First(d => d.Id.Equals(EventType.ReceivedSensorData.GetDescription(), StringComparison.OrdinalIgnoreCase)).Queue;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IMessagingConnectionFactory factory = scope.ServiceProvider.GetRequiredService<IMessagingConnectionFactory>();
        IChannel channel = await factory.CreateChannelAsync(cancellationToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken);
        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            using (LogContext.PushProperty("CorrelationId", ea.BasicProperties.CorrelationId))
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    ReceivedSensorDataEvent? receivedSensorDataEvent = JsonSerializer.Deserialize<ReceivedSensorDataEvent>(message);
                    if (receivedSensorDataEvent is null)
                    {
                        Log.Error("Invalid ReceivedSensorDataEvent message: {Message}", message);
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    using IServiceScope messageScope = _serviceProvider.CreateScope();
                    IInfluxDbService influxDb = messageScope.ServiceProvider.GetRequiredService<IInfluxDbService>();

                    PointData point = PointData
                        .Measurement("agro_sensors")
                        .Tag("sensor_client_id", receivedSensorDataEvent.SensorClientId.ToString())
                        .Field("air_temperature_c", receivedSensorDataEvent.AirTemperatureC)
                        .Field("air_humidity_percent", receivedSensorDataEvent.AirHumidityPercent)
                        .Field("precipitation_mm", receivedSensorDataEvent.PrecipitationMm)
                        .Field("wind_speed_kmh", receivedSensorDataEvent.WindSpeedKmh)
                        .Field("data_quality_score", receivedSensorDataEvent.DataQualityScore)
                        .Field("soil_moisture_percent", receivedSensorDataEvent.SoilMoisturePercent)
                        .Field("soil_ph", receivedSensorDataEvent.SoilPH)
                        .Timestamp(receivedSensorDataEvent.Timestamp, WritePrecision.Ns);
                    influxDb.Write(w => w.WritePoint(point));

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing ReceivedSensorDataEvent with Message {Message}", message);
                    await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                }
            }
        };

        await channel.BasicConsumeAsync(_queue, false, consumer, cancellationToken: cancellationToken);
    }
}
