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





//using InfluxDB.Client;
//using InfluxDB.Client.Core.Flux.Domain;
//using InfluxDB.Client.Writes;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json.Linq;

//namespace AgroSolutions.Ingest.Infrastructure.Services;

//public interface IInfluxDbService
//{
//    void Write(PointData pointData);
//    Task<List<FluxTable>> QueryAsync(string queryValue);
//}

//public class InfluxDbService(IConfiguration configuration) : IInfluxDbService
//{
//    private readonly string _bucket = configuration["InfluxDB:Bucket"]!;
//    private readonly string _org = configuration["InfluxDB:Org"]!;
//    private readonly string _username = configuration["InfluxDB:Username"]!;
//    private readonly string _password = configuration["InfluxDB:Password"]!;
//    private readonly string _token = configuration["InfluxDB:Token"]!;
//    private readonly string _url = configuration["InfluxDB:Url"]!;

//    public void Write(PointData pointData)
//    {
//        //using InfluxDBClient client = new(_url, _token, _password);

//        using InfluxDBClient client = new(new InfluxDBClientOptions(_url)
//        {
//            Bucket = _bucket,
//            Org = _org,
//            Username = _username,
//            Password = _password,
//            Token = _token
//        });
//        using WriteApi writeApi = client.GetWriteApi();
//        writeApi.WritePoint(pointData, _bucket, _org);
//    }

//    public async Task<List<FluxTable>> QueryAsync(string queryValue)
//    {
//        using InfluxDBClient client = new(new InfluxDBClientOptions(_url)
//        {
//            Bucket = _bucket,
//            Org = _org,
//            Username = _username,
//            Password = _password,
//            Token = _token
//        });
//        QueryApi query = client.GetQueryApi();
//        return await query.QueryAsync(queryValue);
//    }
//}
