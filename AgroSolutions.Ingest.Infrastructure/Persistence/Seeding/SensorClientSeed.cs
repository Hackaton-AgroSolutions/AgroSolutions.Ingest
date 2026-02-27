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
                new(Guid.NewGuid(), "sensor-003", "$2a$12$njBb8bEPH./jrDxZykergOP20FXK61ZvtpQ098P2Dsm.TXr6EzAUa", 3), // Password: passSensor003
                new(Guid.NewGuid(), "sensor-004", "$2a$12$J0VLzaL6GReAIwitTAS1BeXfcHbL6Dkpu5UPl0Ie9U8PEDqiAJyEK", 4), // Password: passSensor004
                new(Guid.NewGuid(), "sensor-005", "$2a$12$Bn886ucTafNJ91e98/DxyOn0p1pXbfxUXyInMSCcGw3MAcJUF.Wba", 5), // Password: passSensor005
                new(Guid.NewGuid(), "sensor-006", "$2a$12$AL6MzNWBoRdS7FOF4.RRreRcNpSPXWsaIeu8id4maFMgmYKIk.89G", 6), // Password: passSensor006
                new(Guid.NewGuid(), "sensor-007", "$2a$12$Bd49l32XKEFYqS29ZdPECOqdddMEgB0/kwg/O7pPvPSjOmEl48aiW", 7), // Password: passSensor007
            })
            {
                await context.SensorClients.AddAsync(sensorClient);
                await context.SaveChangesAsync();
                Log.Information("Inserted Sensor Client with ID {SensorClientId}", sensorClient.SensorClientId);
            }
        }
    }
}
