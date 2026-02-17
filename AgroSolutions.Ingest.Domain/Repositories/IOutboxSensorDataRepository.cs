using AgroSolutions.Ingest.Domain.Entitites;

namespace AgroSolutions.Ingest.Domain.Repositories;

public interface IOutboxSensorDataRepository
{
    List<OutboxSensorData> GetPendingOutboxSensorDataTracking();
    Task SaveReceivedSensorDataAsync(OutboxSensorData outboxSensorData, CancellationToken cancellationToken);
}
