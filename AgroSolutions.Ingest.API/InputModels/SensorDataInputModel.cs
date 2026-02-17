namespace AgroSolutions.Ingest.API.InputModels;

public record SensorDataInputModel(
    decimal PrecipitationMm,
    decimal WindSpeedKmh,
    decimal SoilPH,
    decimal AirTemperatureC,
    float AirHumidityPercent,
    float SoilMoisturePercent,
    float DataQualityScore);
