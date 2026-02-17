using Serilog;

namespace AgroSolutions.Ingest.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            Log.Error(ex, "Internal error in the AgroSolutions.Ingest service");
        }
    }
}
