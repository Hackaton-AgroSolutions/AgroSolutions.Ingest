using AgroSolutions.Ingest.API.Extensions;
using AgroSolutions.Ingest.API.InputModels;
using AgroSolutions.Ingest.API.Responses;
using AgroSolutions.Ingest.Application.Commands.SaveSensorData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AgroSolutions.Ingest.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/sensor-datas")]
public class SensorDataController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("save")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(EmptyResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RestResponseWithInvalidFields))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(RestResponse))]
    public async Task<AcceptedResult> Save(SensorDataInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(Save));
        SaveSensorDataCommand command = new(
            User.SensorClientId,
            inputModel.Timestamp,
            Response.CorrelationId(),
            inputModel.PrecipitationMm,
            inputModel.WindSpeedKmh,
            inputModel.SoilPH,
            inputModel.AirTemperatureC,
            inputModel.AirHumidityPercent,
            inputModel.SoilMoisturePercent,
            inputModel.DataQualityScore);
        await _mediator.Send(command);
        return Accepted();
    }
}
