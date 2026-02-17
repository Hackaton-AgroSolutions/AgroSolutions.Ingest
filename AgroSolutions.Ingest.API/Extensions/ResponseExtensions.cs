using Microsoft.Extensions.Primitives;

namespace AgroSolutions.Ingest.API.Extensions;

public static class ResponseExtensions
{
    extension(HttpResponse httpResponse)
    {
        public string? CorrelationId()
        {
            httpResponse.Headers.TryGetValue("X-Correlation-ID", out StringValues correlationId);
            return correlationId.FirstOrDefault();
        }
    }
}
