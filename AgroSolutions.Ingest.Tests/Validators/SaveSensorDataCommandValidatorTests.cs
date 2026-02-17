using AgroSolutions.Ingest.Application.Commands.SaveSensorData;
using AgroSolutions.Ingest.Application.Validators;
using FluentAssertions;
using FluentValidation.Results;

namespace AgroSolutions.Ingest.Tests.Validators;

public class SaveSensorDataCommandValidatorTests
{
    [Fact(DisplayName = "Valid command should pass validation")]
    public void Should_BeValid_WhenCommandIsValid()
    {
        // Arrange
        SaveSensorDataCommand saveSensorDataCommand = new(
            SensorClientId: Guid.NewGuid(),
            Timestamp: DateTime.UtcNow.AddSeconds(-5),
            CorrelationId: Guid.NewGuid(),
            PrecipitationMm: 0.0m,
            WindSpeedKmh: 4.0m,
            SoilPH: 6.8f,
            AirTemperatureC: 31.0m,
            AirHumidityPercent: 66.0f,
            SoilMoisturePercent: 29.0f,
            DataQualityScore: 100);

        // Act
        ValidationResult result = new SaveSensorDataCommandValidator().Validate(saveSensorDataCommand);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = "Invalid command should fail validation")]
    public void Should_BeInvalid_WhenCommandIsInvalid()
    {
        // Arrange
        SaveSensorDataCommand saveSensorDataCommand = new(
            SensorClientId: Guid.NewGuid(),
            Timestamp: DateTime.UtcNow.AddMinutes(5),
            CorrelationId: Guid.NewGuid(),
            PrecipitationMm: -500.0m,
            WindSpeedKmh: -50,
            SoilPH: 23,
            AirTemperatureC: 8000.0m,
            AirHumidityPercent: -126.0f,
            SoilMoisturePercent: 29.0f,
            DataQualityScore: 9500);

        // Act
        ValidationResult result = new SaveSensorDataCommandValidator().Validate(saveSensorDataCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(7);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATATIMESTAMP).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATAPRECIPITATIONMM).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATAWINDSPEEDKMH).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATASOILPH).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATAAIRTEMPERATUREC).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATAAIRHUMIDITYPERCENT).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATAQUALITYSCORE).Should().Be(1);
    }

    [Fact(DisplayName = "Invalid soil pH return hectares validation error")]
    public void Should_HaveInvalidSoilPHError_WhenSoilPHInvalid()
    {
        // Arrange
        SaveSensorDataCommand saveSensorDataCommand = new(
            SensorClientId: Guid.NewGuid(),
            Timestamp: DateTime.Now.AddSeconds(-5),
            CorrelationId: Guid.NewGuid(),
            PrecipitationMm: 0.0m,
            WindSpeedKmh: 4.0m,
            SoilPH: 19,
            AirTemperatureC: 31.0m,
            AirHumidityPercent: 66.0f,
            SoilMoisturePercent: 29.0f,
            DataQualityScore: 100);

        // Act
        ValidationResult result = new SaveSensorDataCommandValidator().Validate(saveSensorDataCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Count(e => e.ErrorMessage == OutboxSensorDataFieldsValidationExtensions.MESSAGE_INVALID_SENSORDATASOILPH).Should().Be(1);
    }
}
