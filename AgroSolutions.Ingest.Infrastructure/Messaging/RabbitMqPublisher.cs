using AgroSolutions.Ingest.Domain.Common;
using AgroSolutions.Ingest.Domain.Events;
using AgroSolutions.Ingest.Domain.Messaging;
using AgroSolutions.Ingest.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using System.Text.Json;

namespace AgroSolutions.Ingest.Infrastructure.Messaging;

public class RabbitMqPublisher(IMessagingConnectionFactory factory, IOptions<RabbitMqOptions> options, IHttpContextAccessor httpContextAccessor) : IEventPublisher
{
    private readonly IMessagingConnectionFactory _factory = factory;
    private readonly RabbitMqOptions _rabbitMqOptions = options.Value;

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken, string? correlationId = default)
    {
        IChannel channel = await _factory.CreateChannelAsync(cancellationToken);
        if (httpContextAccessor.HttpContext is not null)
        {
            httpContextAccessor.HttpContext!.Response.Headers.TryGetValue("X-Correlation-ID", out StringValues stringValues);
            correlationId = stringValues.ToString();
        }
        BasicProperties basicProperties = new() { CorrelationId = correlationId };
        string? routingKey = default;
        byte[]? body = default;

        switch (domainEvent)
        {
            case ReceivedSensorDataEvent receivedSensorDataEvent:
                body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(receivedSensorDataEvent));
                routingKey = GetRoutingKeyByEventType(EventType.ReceivedSensorData);
                Log.Information("Adding the received data from sensor with data {@Data} to the RoutingKey {RoutingKey}.", receivedSensorDataEvent, routingKey);
                break;
        }

        await channel.BasicPublishAsync(_rabbitMqOptions.Exchange, routingKey!, false, basicProperties, body, cancellationToken);
    }

    private string? GetRoutingKeyByEventType(EventType eventType)
        => _rabbitMqOptions.Destinations.FirstOrDefault(d => d.Id.Equals(eventType.GetDescription(), StringComparison.OrdinalIgnoreCase))?.RoutingKey;
}
