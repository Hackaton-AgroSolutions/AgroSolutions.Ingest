using AgroSolutions.Ingest.Application.Queries.AuthenticateSensorClient;
using AgroSolutions.Ingest.Application.Validators;
using FluentAssertions;
using FluentValidation.Results;

namespace AgroSolutions.Ingest.Tests.Validators;

public class AuthenticateSensorClientQueryValidatorTests
{
    [Fact(DisplayName = "Valid query should pass validation")]
    public void Should_BeValid_WhenQueryIsValid()
    {
        // Arrange
        AuthenticateSensorClientQuery authenticateSensorClientQuery = new("sensor-543", "validPassword");

        // Act
        ValidationResult result = new AuthenticateSensorClientQueryValidator().Validate(authenticateSensorClientQuery);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Invalid query should fail validation when client id is invalid")]
    public void Should_BeInvalid_WhenClientIdIsInvalid()
    {
        // Arrange
        AuthenticateSensorClientQuery authenticateSensorClientQuery = new("invalid client id", "validPassword");

        // Act
        ValidationResult result = new AuthenticateSensorClientQueryValidator().Validate(authenticateSensorClientQuery);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Count(e => e.ErrorMessage == SensorClientFieldsValidationExtensions.MESSAGE_INVALID_CLIENTID).Should().Be(1);
    }

    [Fact(DisplayName = "Invalid query should fail validation when client id is empty")]
    public void Should_BeInvalid_WhenClientIdIsEmpty()
    {
        // Arrange
        AuthenticateSensorClientQuery authenticateSensorClientQuery = new(string.Empty, "validPassword");

        // Act
        ValidationResult result = new AuthenticateSensorClientQueryValidator().Validate(authenticateSensorClientQuery);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Count(e => e.ErrorMessage == SensorClientFieldsValidationExtensions.MESSAGE_EMPTY_CLIENTID).Should().Be(1);
    }
}
