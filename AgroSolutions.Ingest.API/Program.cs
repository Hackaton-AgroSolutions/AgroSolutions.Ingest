using AgroSolutions.Ingest.API.Filters;
using AgroSolutions.Ingest.API.Middleware;
using AgroSolutions.Ingest.Application;
using AgroSolutions.Ingest.Infrastructure;
using AgroSolutions.Ingest.Infrastructure.Messaging;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using AgroSolutions.Ingest.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("service_name", "agro-solution-ingest-api")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] (CorrelationId={CorrelationId}) {Message:lj} {NewLine}{Exception}")
    .WriteTo.GrafanaLoki("http://loki:3100")
    .CreateLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services
    .AddControllers(options => options.Filters.Add<RestResponseFilter>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Host.UseSerilog();

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "AgroSolutions - Ingest",
        Description = "Developed by Mário Guilherme de Andrade Rodrigues",
        Version = "v1",
        Contact = new()
        {
            Name = "Mário Guilherme de Andrade Rodrigues",
            Email = "marioguilhermedev@gmail.com"
        },
        License = new()
        {
            Name = "MIT",
            Url = new("https://opensource.org/licenses/MIT")
        }
    });

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
    });
});
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

using AsyncServiceScope asyncServiceScope = app.Services.CreateAsyncScope();
IServiceProvider services = asyncServiceScope.ServiceProvider;

#region Performs the initial insertion of client sensors into the database in the development environment.
try
{
    AgroSolutionsIngestDbContext context = services.GetRequiredService<AgroSolutionsIngestDbContext>();
    await SensorClientSeed.SeedDatabaseAsync(context);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error during initial insertion into the development environment.");
}
#endregion

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

app.UseMetricServer();
app.MapGet("/metrics", () => "test_metric 1");
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapMetrics();

app.MapHealthChecks("/health");

await app.RunAsync();

Log.CloseAndFlush();
