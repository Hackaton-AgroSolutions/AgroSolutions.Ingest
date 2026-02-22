using InfluxDB.Client;

namespace AgroSolutions.Ingest.Functions.Interfaces;

public interface IInfluxDbService
{
    void Write(Action<WriteApi> action);
}
