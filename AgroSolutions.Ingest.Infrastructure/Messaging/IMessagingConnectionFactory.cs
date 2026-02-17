using RabbitMQ.Client;

namespace AgroSolutions.Ingest.Infrastructure.Messaging;

public interface IMessagingConnectionFactory
{
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken);
}
