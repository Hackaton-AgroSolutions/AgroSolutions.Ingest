using System.Security.Claims;

namespace AgroSolutions.Ingest.API.Extensions;

public static class ClaimsExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public Guid SensorClientId => Guid.Parse(principal.Claims.First(c => c.Type == "SensorClientId").Value);
        public int FieldId => int.Parse(principal.Claims.First(c => c.Type == "FieldId").Value);
    }
}
