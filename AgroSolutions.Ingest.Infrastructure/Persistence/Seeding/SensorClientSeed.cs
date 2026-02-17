using AgroSolutions.Ingest.Domain.Entitites;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Ingest.Infrastructure.Persistence.Seeding;

public static class SensorClientSeed
{
    public static async Task SeedDatabaseAsync(AgroSolutionsIngestDbContext context)
    {
        if (!await context.SensorClients.AnyAsync(sc => sc.ClientId == "sensor-001" || sc.ClientId == "sensor-002"))
        {
            foreach (SensorClient sensorClient in new SensorClient[]
            {
                new(Guid.NewGuid(), "sensor-001", "$2a$12$4AEaYq3LAX6AExKUruehWOCFfr0jfPgDt/xr6lqwnnNM4t84JiOTS", 1), // Password: passSensor001
                new(Guid.NewGuid(), "sensor-002", "$2a$12$4dXdDPoJRyyzuEx/ZRiiieIAR0Mei2Vfvmi8YyOZK8PefQBpXkYn2", 2) // Password: passSensor002
            })
            {
                await context.SensorClients.AddAsync(sensorClient);
                await context.SaveChangesAsync();
            }
        }
    }
}
