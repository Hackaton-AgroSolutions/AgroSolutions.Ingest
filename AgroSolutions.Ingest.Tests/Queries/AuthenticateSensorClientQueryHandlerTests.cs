using AgroSolutions.Ingest.Application.Queries.AuthenticateSensorClient;
using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Notifications;
using AgroSolutions.Ingest.Domain.Service;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using FluentAssertions;
using Moq;

namespace AgroSolutions.Ingest.Tests.Queries;

public class AuthenticateSensorClientQueryHandlerTests
{
    private readonly Mock<INotificationContext> _notificationContext = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ISensorClientAuthService> _sensorClientAuthService = new();
    private readonly AuthenticateSensorClientQueryHandler _queryHandler;

    public AuthenticateSensorClientQueryHandlerTests()
    {
        _queryHandler = new(
            _unitOfWork.Object,
            _sensorClientAuthService.Object,
            _notificationContext.Object
        );
    }

    [Fact(DisplayName = "Should return token when email and password matchs")]
    public async Task Should_ReturnToken_WhenClientIdAndClientSecretdMatchs()
    {
        // Arrange
        AuthenticateSensorClientQuery authenticateSensorClientQuery = new("sensor-001", "passSensor001");
        SensorClient sensorClientDb = new(authenticateSensorClientQuery.ClientId, "$2a$12$XH0x/BDz0SmEKP4Q55zfuemM9.fDESJF1h2ock4LbD9oJuaEpW5Ke", 1);
        _unitOfWork.Setup(u => u.SensorClients.GetByClientIdNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(sensorClientDb);
        _sensorClientAuthService.Setup(a => a.GenerateToken(sensorClientDb)).Returns("eyJb");

        // Act
        AuthenticateSensorClientQueryResult authenticateSensorClientQueryResult = (await _queryHandler.Handle(authenticateSensorClientQuery, CancellationToken.None))!;

        // Assert
        authenticateSensorClientQueryResult.Should().NotBeNull();
        authenticateSensorClientQueryResult.Token.Should().NotBeNullOrEmpty();
        authenticateSensorClientQueryResult.Token.Should().Be("eyJb");
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        _sensorClientAuthService.Verify(a => a.GenerateToken(It.IsAny<SensorClient>()), Times.Once);
        _unitOfWork.Verify(u => u.SensorClients.GetByClientIdNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should notify and return null when client ind and client secret don't matches")]
    public async Task Should_ReturnNullAndNotify_WhenClientIdAndClientSecretDontMatches()
    {
        // Arrange
        AuthenticateSensorClientQuery authenticateSensorClientQuery = new("sensor-001", "invalidPassword");
        _unitOfWork.Setup(u => u.SensorClients.GetByClientIdNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((SensorClient?)null);

        // Act
        AuthenticateSensorClientQueryResult? authenticateSensorClientQueryResult = await _queryHandler.Handle(authenticateSensorClientQuery, CancellationToken.None);

        // Assert
        authenticateSensorClientQueryResult.Should().BeNull();
        authenticateSensorClientQueryResult.Should().BeNull();
        authenticateSensorClientQueryResult?.Token?.Should().BeNullOrEmpty();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        _unitOfWork.Verify(u => u.SensorClients.GetByClientIdNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _sensorClientAuthService.Verify(a => a.GenerateToken(It.IsAny<SensorClient>()), Times.Never);
    }
}
