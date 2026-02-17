using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Notifications;
using AgroSolutions.Ingest.Domain.Service;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using MediatR;
using Serilog;

namespace AgroSolutions.Ingest.Application.Queries.AuthenticateSensorClient;

public class AuthenticateSensorClientQueryHandler(IUnitOfWork unitOfWork,
    ISensorClientAuthService sensorClientAuthService,
    INotificationContext notification) : IRequestHandler<AuthenticateSensorClientQuery, AuthenticateSensorClientQueryResult?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISensorClientAuthService _sensorClientAuthService = sensorClientAuthService;
    private readonly INotificationContext _notification = notification;

    public async Task<AuthenticateSensorClientQueryResult?> Handle(AuthenticateSensorClientQuery request, CancellationToken cancellationToken)
    {
        Log.Information("Starting the authentication of sensor.");

        SensorClient? sensorClient = await _unitOfWork.SensorClients.GetByClientIdNoTrackingAsync(request.ClientId, cancellationToken);
        if (sensorClient is null || !BCrypt.Net.BCrypt.Verify(request.ClientSecret, sensorClient.ClientSecret))
        {
            Log.Warning("Non-existent client id and/or client secret combination.");
            _notification.AddNotification(NotificationType.InvalidSensorCredentials);
            return null;
        }

        Log.Information("Generating a token for the sensor client with ID {SensorClientId}.", sensorClient.SensorClientId);
        string token = _sensorClientAuthService.GenerateToken(sensorClient);

        Log.Information("Finished the authentication of sensor.");
        return new(token);
    }
}
