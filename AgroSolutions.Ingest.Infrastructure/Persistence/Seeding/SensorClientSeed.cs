using AgroSolutions.Ingest.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AgroSolutions.Ingest.Infrastructure.Persistence.Seeding;

public static class SensorClientSeed
{
    public static async Task SeedDatabaseAsync(AgroSolutionsIngestDbContext context)
    {
        if (!await context.SensorClients.AnyAsync())
        {
            foreach (SensorClient sensorClient in new SensorClient[]
            {
                new(Guid.NewGuid(), "sensor-001", "$2a$12$4AEaYq3LAX6AExKUruehWOCFfr0jfPgDt/xr6lqwnnNM4t84JiOTS", 1), // Password: passSensor001
                new(Guid.NewGuid(), "sensor-002", "$2a$12$4dXdDPoJRyyzuEx/ZRiiieIAR0Mei2Vfvmi8YyOZK8PefQBpXkYn2", 2), // Password: passSensor002
                new(Guid.NewGuid(), "sensor-003", "$2a$12$7OtRv/kZ5o0IvpLvd0R8mu5f5C5kfcA.GDYRZ9lRhqqSRg61nk5bO", 3), // Password: passSensor003
                new(Guid.NewGuid(), "sensor-004", "$2a$12$4dXdDPoJRyyzuEx/ZRiiieIAR0Mei2Vfvmi8YyOZK8PefQBpXkYn2", 4), // Password: passSensor004
                new(Guid.NewGuid(), "sensor-005", "$2a$12$7OtRv/kZ5o0IvpLvd0R8mu5f5C5kfcA.GDYRZ9lRhqqSRg61nk5bO", 5), // Password: passSensor005
                new(Guid.NewGuid(), "sensor-006", "$2a$12$mTw85pFcfdq3razNgwq7meErx2vtDnUrD.fc20QbrbcPy2tY35yri", 6), // Password: passSensor006
                new(Guid.NewGuid(), "sensor-007", "$2a$12$tI28NigfYGN4Q17PyZmFsezo2r4SHvksnZcE6r6wZzjMf1Tlcj7.O", 7), // Password: passSensor007
            })
            {
                await context.SensorClients.AddAsync(sensorClient);
                await context.SaveChangesAsync();
                Log.Information("Inserted Sensor Client with ID {SensorClientId}", sensorClient.SensorClientId);
            }
        }
    }
}
