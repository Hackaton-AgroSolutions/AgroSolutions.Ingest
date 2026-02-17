using AgroSolutions.Ingest.Domain.Entitites;

namespace AgroSolutions.Ingest.Domain.Repositories;

public interface ISensorClientRepository
{
    Task<SensorClient?> GetByClientIdNoTrackingAsync(string clientId, CancellationToken cancellationToken);
}
