using AgroSolutions.Ingest.Domain.Enums;

namespace AgroSolutions.Ingest.Domain.Entitites;

public class OutboxSensorData
{
    public Guid OutboxSensorDataId { get; private set; }
    public Guid Correlationid { get; private set; }
    public string Payload { get; private set; }
    public OutboxSensorDataStatus Status { get; private set; } = OutboxSensorDataStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public OutboxSensorData()
    {
    }

    public OutboxSensorData(Guid correlationId, string payload)
    {
        Correlationid = correlationId;
        Payload = payload;
    }

    public void MarkAsProcessed() => Status = OutboxSensorDataStatus.Processed;

    public void MarkAsFailed() => Status = OutboxSensorDataStatus.Failed;
}
