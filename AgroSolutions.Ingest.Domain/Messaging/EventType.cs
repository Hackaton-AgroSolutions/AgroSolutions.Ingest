using System.ComponentModel;

namespace AgroSolutions.Ingest.Domain.Messaging;

public enum EventType : byte
{
    [Description("RECEIVED_SENSOR_DATA")] ReceivedSensorData
}
