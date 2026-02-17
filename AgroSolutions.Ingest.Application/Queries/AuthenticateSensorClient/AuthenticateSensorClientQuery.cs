using MediatR;

namespace AgroSolutions.Ingest.Application.Queries.AuthenticateSensorClient;

public record AuthenticateSensorClientQuery(string ClientId, string ClientSecret) : IRequest<AuthenticateSensorClientQueryResult?>;
