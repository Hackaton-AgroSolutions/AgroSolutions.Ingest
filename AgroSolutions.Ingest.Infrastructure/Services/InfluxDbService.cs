using InfluxDB.Client;
using Microsoft.Extensions.Configuration;

namespace AgroSolutions.Ingest.Infrastructure.Services;

public interface IInfluxDbService
{
    void Write(Action<WriteApi> action);
}

public class InfluxDbService(IConfiguration configuration) : IInfluxDbService
{
    public void Write(Action<WriteApi> action)
    {
        using InfluxDBClient client = new(new InfluxDBClientOptions(configuration["InfluxDB:Url"])
        {
            Bucket = configuration["InfluxDB:Bucket"],
            Org = configuration["InfluxDB:Org"],
            Username = configuration["InfluxDB:Username"],
            Password = configuration["InfluxDB:Password"],
            Token = configuration["InfluxDB:Token"]
        });
        using WriteApi write = client.GetWriteApi();
        action(write);
    }
}
