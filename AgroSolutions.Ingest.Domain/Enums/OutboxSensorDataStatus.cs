namespace AgroSolutions.Ingest.Domain.Enums;

public enum OutboxSensorDataStatus : byte
{
    Pending = 0,
    Processed = 1,
    Failed = 2
}
