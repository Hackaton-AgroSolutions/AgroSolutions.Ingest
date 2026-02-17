using InfluxDB.Client;
using Microsoft.Extensions.Configuration;

namespace AgroSolutions.Ingest.Infrastructure.Services;

public interface IInfluxDbService
{
    void Write(Action<WriteApi> action);
    Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action);
}

public class InfluxDbService(IConfiguration configuration) : IInfluxDbService
{
    private readonly string _token = configuration["InfluxDB:Token"]!;

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

    public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
    {
        using InfluxDBClient client = new(configuration["InfluxDB:Url"], _token);
        QueryApi query = client.GetQueryApi();
        return await action(query);
    }
}
