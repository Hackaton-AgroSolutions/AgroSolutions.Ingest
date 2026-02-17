using AgroSolutions.Ingest.Domain.Repositories;

namespace AgroSolutions.Ingest.Infrastructure.Persistence;

public class UnitOfWork(AgroSolutionsIngestDbContext dbContext,
    ISensorClientRepository sensorClients,
    IOutboxSensorDataRepository outboxSensorDatas) : IUnitOfWork
{
    private readonly AgroSolutionsIngestDbContext _dbContext = dbContext;

    public ISensorClientRepository SensorClients => sensorClients;
    public IOutboxSensorDataRepository OutboxSensorDatas => outboxSensorDatas;

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _dbContext.Dispose();
    }
}
