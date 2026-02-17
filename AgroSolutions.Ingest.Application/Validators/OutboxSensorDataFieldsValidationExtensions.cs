using FluentValidation;

namespace AgroSolutions.Ingest.Application.Validators;

public static class OutboxSensorDataFieldsValidationExtensions
{
    public const string MESSAGE_EMPTY_SENSORDATASENSORCLIENTID = "The sensor client id needs to be provided";
    public const string MESSAGE_EMPTY_SENSORDATACORRELATIONID = "The correlation id needs to be provided";
    public const string MESSAGE_INVALID_SENSORDATAPRECIPITATIONMM = "The precipitation in mm cannot be less than or equal to 0";
    public const string MESSAGE_INVALID_SENSORDATAWINDSPEEDKMH = "The wind speed in km/h cannot be less than or equal to 0";
    public const string MESSAGE_INVALID_SENSORDATASOILPH = "The soil pH cannot be less than 0 or greater than 14";
    public const string MESSAGE_INVALID_SENSORDATATIMESTAMP = "The timestamp cannot be in the future";
    public const string MESSAGE_INVALID_SENSORDATAAIRTEMPERATUREC = "The air temperature should be between -20°C and 80°C";
    public const string MESSAGE_INVALID_SENSORDATAAIRHUMIDITYPERCENT = "The air humidity should be between 0% and 100%";
    public const string MESSAGE_INVALID_SENSORDATASOILMOISTUREPERCENT = "The soil moisture should be between 0% and 100%";
    public const string MESSAGE_INVALID_SENSORDATAQUALITYSCORE = "The data quality score must be between 0 and 100";

    extension<T>(IRuleBuilder<T, Guid> rule)
    {
        public IRuleBuilderOptions<T, Guid> ValidSensorClientId() => rule
            .NotEmpty().WithMessage(MESSAGE_EMPTY_SENSORDATASENSORCLIENTID);

        public IRuleBuilderOptions<T, Guid> ValidSensorDataCorrelationId() => rule
            .NotEmpty().WithMessage(MESSAGE_EMPTY_SENSORDATACORRELATIONID);
    }

    extension<T>(IRuleBuilder<T, decimal> rule)
    {
        public IRuleBuilderOptions<T, decimal> ValidSensorDataPrecipitationMm() => rule
            .GreaterThan(0).WithMessage(MESSAGE_INVALID_SENSORDATAPRECIPITATIONMM);

        public IRuleBuilderOptions<T, decimal> ValidSensorDataWindSpeedKmh() => rule
            .GreaterThan(0).WithMessage(MESSAGE_INVALID_SENSORDATAWINDSPEEDKMH);

        public IRuleBuilderOptions<T, decimal> ValidSensorAirTemperatureC() => rule
            .InclusiveBetween(-20, 80).WithMessage(MESSAGE_INVALID_SENSORDATAAIRTEMPERATUREC);
    }

    extension<T>(IRuleBuilder<T, float> rule)
    {
        public IRuleBuilderOptions<T, float> ValidSensorDataSoilPH() => rule
            .InclusiveBetween(0, 14).WithMessage(MESSAGE_INVALID_SENSORDATASOILPH);

        public IRuleBuilderOptions<T, float> ValidSensorAirHumidityPercent() => rule
            .InclusiveBetween(0, 100).WithMessage(MESSAGE_INVALID_SENSORDATAAIRHUMIDITYPERCENT);

        public IRuleBuilderOptions<T, float> ValidSensorSoilMoisturePercent() => rule
            .InclusiveBetween(0, 100).WithMessage(MESSAGE_INVALID_SENSORDATASOILMOISTUREPERCENT);

        public IRuleBuilderOptions<T, float> ValidSensorDataQualityScore() => rule
            .InclusiveBetween(0, 100).WithMessage(MESSAGE_INVALID_SENSORDATAQUALITYSCORE);
    }

    extension<T>(IRuleBuilder<T, DateTime> rule)
    {
        public IRuleBuilderOptions<T, DateTime> ValidSensorDataTimestamp() => rule
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(MESSAGE_INVALID_SENSORDATATIMESTAMP);
    }
}
