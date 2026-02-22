using AgroSolutions.Ingest.Functions;
using AgroSolutions.Ingest.Functions.Interfaces;
using AgroSolutions.Ingest.Functions.Services;
using AgroSolutions.Ingest.Infrastructure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("Messaging"));

builder.Services.AddSingleton<IInfluxDbService>(sp => new InfluxDbService(builder.Configuration));
builder.Services.AddSingleton<IRabbitConnectionProvider, RabbitConnectionProvider>();
builder.Services.AddScoped<IMessagingConnectionFactory, RabbitChannelFactory>();

IHost host = builder.Build();

using AsyncServiceScope asyncServiceScope = host.Services.CreateAsyncScope();
IServiceProvider services = asyncServiceScope.ServiceProvider;

#region Ensures the creation of the exchange, queues, and message binds at startup.
try
{
    IMessagingConnectionFactory factory = services.GetRequiredService<IMessagingConnectionFactory>();
    IOptions<RabbitMqOptions> options = services.GetRequiredService<IOptions<RabbitMqOptions>>();
    await RabbitMqConnection.InitializeAsync(await factory.CreateChannelAsync(CancellationToken.None), options.Value);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error during messaging initialization.");
}
#endregion

host.Run();
