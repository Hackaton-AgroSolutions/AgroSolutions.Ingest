using AgroSolutions.Ingest.Application.Commands.SaveSensorData;
using AgroSolutions.Ingest.Application.Notifications;
using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Notifications;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Moq;

namespace AgroSolutions.Ingest.Tests.Commands;

public class SaveSensorDataCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly SaveSensorDataCommandHandler _commandHandler;

    public SaveSensorDataCommandHandlerTests()
    {
        _commandHandler = new(_unitOfWork.Object);
    }

    [Fact(DisplayName = "Should register sensor data and return unit when everthing it is ok")]
    public async Task Should_SaveSensorDataReturnUnit_WhenEverythingItIsOk()
    {
        // Arrange
        SaveSensorDataCommand saveSensorDataCommand = new(
            SensorClientId: Guid.NewGuid(),
            Timestamp: DateTime.Now.AddSeconds(-5),
            CorrelationId: Guid.NewGuid(),
            PrecipitationMm: 0.0m,
            WindSpeedKmh: 4.0m,
            SoilPH: 6.8f,
            AirTemperatureC: 31.0m,
            AirHumidityPercent: 66.0f,
            SoilMoisturePercent: 29.0f,
            DataQualityScore: 100);
        _unitOfWork.Setup(u => u.OutboxSensorDatas.SaveSensorDataAsync(It.IsAny<OutboxSensorData>(), It.IsAny<CancellationToken>()));

        // Act
        Unit? unit = await _commandHandler.Handle(saveSensorDataCommand, CancellationToken.None);

        // Assert
        unit.Should().NotBeNull();
        unit.Should().Be(Unit.Value);
        _unitOfWork.Verify(u => u.OutboxSensorDatas.SaveSensorDataAsync(It.IsAny<OutboxSensorData>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
