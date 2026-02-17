using AgroSolutions.Ingest.Application.Validators;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Ingest.Application.Queries.AuthenticateSensorClient;

public class AuthenticateSensorClientQueryValidator : AbstractValidator<AuthenticateSensorClientQuery>
{
    public AuthenticateSensorClientQueryValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(AuthenticateSensorClientQueryValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(q => q.ClientId).ValidClientId();
        RuleFor(q => q.ClientSecret).ValidClientSecret();
    }
}
