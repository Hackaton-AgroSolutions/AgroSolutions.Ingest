namespace AgroSolutions.Ingest.API.Responses;

public record RestResponse(object? Data = default)
{
    public IEnumerable<string> Notifications { get; init; } = [];
}
