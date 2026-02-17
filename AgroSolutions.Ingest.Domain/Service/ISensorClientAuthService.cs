using AgroSolutions.Ingest.Domain.Entitites;

namespace AgroSolutions.Ingest.Domain.Service;

public interface ISensorClientAuthService
{
    public string GenerateToken(SensorClient sensorClient);
}
