using MediatR;

namespace AgroSolutions.Ingest.Application.Commands.SaveSensorData;

public record SaveSensorDataCommand(
    Guid SensorClientId,
    int FieldId,
    DateTime Timestamp,
    Guid CorrelationId,
    decimal PrecipitationMm,
    decimal WindSpeedKmh,
    float SoilPH,
    decimal AirTemperatureC,
    float AirHumidityPercent,
    float SoilMoisturePercent,
    float DataQualityScore) : IRequest<Unit?>;
