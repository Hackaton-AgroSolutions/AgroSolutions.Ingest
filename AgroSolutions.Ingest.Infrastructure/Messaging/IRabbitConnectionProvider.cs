using RabbitMQ.Client;

namespace AgroSolutions.Ingest.Infrastructure.Messaging;

public interface IRabbitConnectionProvider
{
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken);
}
