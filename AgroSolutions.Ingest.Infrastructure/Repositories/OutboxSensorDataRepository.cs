using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Enums;
using AgroSolutions.Ingest.Domain.Repositories;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using MongoDB.Driver.Linq;

namespace AgroSolutions.Ingest.Infrastructure.Repositories;

public class OutboxSensorDataRepository(AgroSolutionsIngestDbContext dbContext) : IOutboxSensorDataRepository
{
    private readonly AgroSolutionsIngestDbContext _dbContext = dbContext;

    public List<OutboxSensorData> GetPendingOutboxSensorDataTracking() => [.. _dbContext.OutboxSensorDatas.Where(o => o.Status == OutboxSensorDataStatus.Pending)];

    public async Task SaveSensorDataAsync(OutboxSensorData outboxSensorData, CancellationToken cancellationToken)
    {
        await _dbContext.OutboxSensorDatas.AddAsync(outboxSensorData, cancellationToken);
    }
}
