using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Events;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using MediatR;
using Serilog;
using System.Text.Json;

namespace AgroSolutions.Ingest.Application.Commands.SaveSensorData;

public class SaveSensorDataCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<SaveSensorDataCommand, Unit?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Unit?> Handle(SaveSensorDataCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Starting the save of data from sensor.");

        ReceivedSensorDataEvent receivedSensorDataEvent = new(
            request.SensorClientId,
            request.CorrelationId,
            request.FieldId,
            request.PrecipitationMm,
            request.WindSpeedKmh,
            request.SoilPH,
            request.AirTemperatureC,
            request.AirHumidityPercent,
            request.SoilMoisturePercent,
            request.DataQualityScore);

        OutboxSensorData outboxSensorData = new(request.CorrelationId, JsonSerializer.Serialize(receivedSensorDataEvent));

        Log.Information("Saving sensor data to the database for future processing. Received {@Data}.", receivedSensorDataEvent);
        await _unitOfWork.OutboxSensorDatas.SaveSensorDataAsync(outboxSensorData, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Log.Information("Finished the save of data from sensor.");
        return Unit.Value;
    }
}
