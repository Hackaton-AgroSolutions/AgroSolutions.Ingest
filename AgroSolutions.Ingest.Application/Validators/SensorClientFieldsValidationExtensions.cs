using FluentValidation;

namespace AgroSolutions.Ingest.Application.Validators;

public static class SensorClientFieldsValidationExtensions
{
    public const string MESSAGE_EMPTY_CLIENTID = "The client id needs to be provided";
    public const string MESSAGE_INVALID_CLIENTID = "The client id is invalid";
    public const string MESSAGE_EMPTY_CLIENTSECRET = "The client secret needs to be provided";
    public const string MESSAGE_INVALID_LENGTH_CLIENTSECRET = "The client secret is invalid";

    extension<T>(IRuleBuilder<T, string> rule)
    {
        public IRuleBuilderOptions<T, string> ValidClientId() => rule
            .NotEmpty().WithMessage(MESSAGE_EMPTY_CLIENTID)
            .Matches("^sensor-[0-9]+$").WithMessage(MESSAGE_INVALID_CLIENTID);

        public IRuleBuilderOptions<T, string> ValidClientSecret() => rule
            .NotEmpty().WithMessage(MESSAGE_EMPTY_CLIENTSECRET)
            .MaximumLength(128).WithMessage(MESSAGE_INVALID_LENGTH_CLIENTSECRET);
    }
}
