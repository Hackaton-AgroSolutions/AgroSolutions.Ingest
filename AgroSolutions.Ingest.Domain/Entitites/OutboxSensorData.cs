namespace AgroSolutions.Ingest.Domain.Entitites;

public class OutboxSensorData(string payload)
{
    public Guid OutboxSensorDataId { get; private set; }
    public string Payload { get; private set; } = payload;
    public string Status { get; private set; } = "Pending";
    public Guid Correlationid { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void MarkAsProcessed()
    {
        Status = "Processed";
    }

    public void MarkAsFailed()
    {
        Status = "Failed";
    }
}
