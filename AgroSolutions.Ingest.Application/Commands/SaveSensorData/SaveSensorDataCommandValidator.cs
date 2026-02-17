using AgroSolutions.Ingest.Application.Validators;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Ingest.Application.Commands.SaveSensorData;

public class SaveSensorDataCommandValidator : AbstractValidator<SaveSensorDataCommand>
{
    public SaveSensorDataCommandValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(SaveSensorDataCommandValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.SensorClientId).ValidSensorClientId();
        RuleFor(c => c.Timestamp).ValidSensorDataTimestamp();
        RuleFor(c => c.CorrelationId).ValidSensorDataCorrelationId();
        RuleFor(c => c.PrecipitationMm).ValidSensorDataPrecipitationMm();
        RuleFor(c => c.WindSpeedKmh).ValidSensorDataWindSpeedKmh();
        RuleFor(c => c.SoilPH).ValidSensorDataSoilPH();
        RuleFor(c => c.AirTemperatureC).ValidSensorAirTemperatureC();
        RuleFor(c => c.AirHumidityPercent).ValidSensorAirHumidityPercent();
        RuleFor(c => c.SoilMoisturePercent).ValidSensorSoilMoisturePercent();
        RuleFor(c => c.DataQualityScore).ValidSensorDataQualityScore();
    }
}
