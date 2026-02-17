namespace AgroSolutions.Ingest.API.InputModels;

public record SensorDataInputModel(
    DateTime Timestamp,
    decimal PrecipitationMm,
    decimal WindSpeedKmh,
    float SoilPH,
    decimal AirTemperatureC,
    float AirHumidityPercent,
    float SoilMoisturePercent,
    float DataQualityScore);
