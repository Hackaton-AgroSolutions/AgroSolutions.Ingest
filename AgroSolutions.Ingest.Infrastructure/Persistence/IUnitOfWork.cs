using AgroSolutions.Ingest.Domain.Repositories;

namespace AgroSolutions.Ingest.Infrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    ISensorClientRepository SensorClients { get; }
    IOutboxSensorDataRepository OutboxReceivedSensorDatas { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
