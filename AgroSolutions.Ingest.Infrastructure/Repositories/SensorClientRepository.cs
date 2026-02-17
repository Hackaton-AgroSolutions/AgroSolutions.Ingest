using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Repositories;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Ingest.Infrastructure.Repositories;

public class SensorClientRepository(AgroSolutionsIngestDbContext dbContext) : ISensorClientRepository
{
    private readonly AgroSolutionsIngestDbContext _dbContext = dbContext;

    public Task<SensorClient?> GetByClientIdNoTrackingAsync(string clientId, CancellationToken cancellationToken) => _dbContext.SensorClients
        .AsNoTracking()
        .FirstOrDefaultAsync(s => s.ClientId == clientId, cancellationToken);
}
