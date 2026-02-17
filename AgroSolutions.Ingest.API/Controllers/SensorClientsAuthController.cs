using AgroSolutions.Ingest.API.InputModels;
using AgroSolutions.Ingest.API.Responses;
using AgroSolutions.Ingest.Application.Queries.AuthenticateSensorClient;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AgroSolutions.Ingest.API.Controllers;

[ApiController]
[Route("api/v1/sensor-clients")]
public class SensorClientsAuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("auth")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RestResponseWithInvalidFields))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(RestResponse))]
    public async Task<OkObjectResult> AuthenticateSensorClient(GetSensorTokenInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(AuthenticateSensorClient));
        AuthenticateSensorClientQuery query = new(inputModel.ClientId, inputModel.ClientSecret);
        AuthenticateSensorClientQueryResult? authenticateSensorClientQueryResult = await _mediator.Send(query);
        return Ok(authenticateSensorClientQueryResult);
    }
}
