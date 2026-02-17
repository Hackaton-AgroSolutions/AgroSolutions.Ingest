using Microsoft.Extensions.Primitives;

namespace AgroSolutions.Ingest.API.Extensions;

public static class ResponseExtensions
{
    extension(HttpResponse httpResponse)
    {
        public Guid CorrelationId()
        {
            httpResponse.Headers.TryGetValue("X-Correlation-ID", out StringValues correlationId);
            return Guid.Parse(correlationId.First()!);
        }
    }
}
