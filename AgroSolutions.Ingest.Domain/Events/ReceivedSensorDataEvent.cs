using AgroSolutions.Ingest.Domain.Common;

namespace AgroSolutions.Ingest.Domain.Events;

public record ReceivedSensorDataEvent(
    Guid SensorClientId,
    decimal PrecipitationMm,
    decimal WindSpeedKmh,
    float SoilPH,
    decimal AirTemperatureC,
    float AirHumidityPercent,
    float SoilMoisturePercent,
    float DataQualityScore) : IDomainEvent
{
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
}
