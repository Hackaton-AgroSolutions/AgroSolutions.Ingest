using AgroSolutions.Ingest.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace AgroSolutions.Ingest.Infrastructure.Persistence;

public class AgroSolutionsIngestDbContext(DbContextOptions<AgroSolutionsIngestDbContext> options) : DbContext(options)
{
    public required DbSet<SensorClient> SensorClients { get; set; }
    public required DbSet<OutboxSensorData> OutboxSensorDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorClient>().ToCollection("sensor-credentials");
        modelBuilder.Entity<OutboxSensorData>().ToCollection("outbox-sensor-datas");
    }
}
