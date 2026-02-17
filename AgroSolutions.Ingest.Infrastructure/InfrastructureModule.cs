using AgroSolutions.Ingest.Domain.Messaging;
using AgroSolutions.Ingest.Domain.Repositories;
using AgroSolutions.Ingest.Domain.Service;
using AgroSolutions.Ingest.Infrastructure;
using AgroSolutions.Ingest.Infrastructure.BackgroundServices;
using AgroSolutions.Ingest.Infrastructure.Messaging;
using AgroSolutions.Ingest.Infrastructure.Persistence;
using AgroSolutions.Ingest.Infrastructure.Repositories;
using AgroSolutions.Ingest.Infrastructure.Services;
using AgroSolutions.Ingest.Infrastructure.Subscribers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

namespace AgroSolutions.Ingest.Infrastructure;

public static class InfrastructureModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services
                .AddMessageBroker(configuration)
                .AddOutboxProcessor()
                .AddAuthentication(configuration)
                .AddPersistence(configuration)
                .AddInfluxDb(configuration)
                .AddRepositories()
                .AddUnitOfWork();

            return services;
        }

        private IServiceCollection AddMessageBroker(IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("Messaging"));

            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IRabbitConnectionProvider, RabbitConnectionProvider>();
            services.AddScoped<IMessagingConnectionFactory, RabbitChannelFactory>();
            services.AddScoped<IEventPublisher, RabbitMqPublisher>();

            return services;
        }

        private IServiceCollection AddOutboxProcessor()
        {
            services.AddHostedService<OutboxSensorDataProcessor>();

            return services;
        }

        private IServiceCollection AddAuthentication(IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                    };
                });

            services.AddScoped<ISensorClientAuthService, SensorClientAuthService>();

            return services;
        }

        private IServiceCollection AddPersistence(IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                string mongoConnectionString = configuration["MongoDb:ConnectionString"]!;
                return new MongoClient(mongoConnectionString);
            });
            //services.AddScoped(sp =>
            //{
            //    IMongoClient client = sp.GetRequiredService<IMongoClient>();
            //    string mongoDatabaseName = configuration["MongoDb:Database"]!;
            //    return client.GetDatabase(mongoDatabaseName);
            //});
            services.AddDbContext<AgroSolutionsIngestDbContext>(options =>
            {
                string mongoConnectionString = configuration["MongoDb:ConnectionString"]!;
                string mongoDatabaseName = configuration["MongoDb:Database"]!;
                options.UseMongoDB(mongoConnectionString, mongoDatabaseName);
            });

            return services;
        }

        private IServiceCollection AddRepositories()
        {
            services.AddScoped<ISensorClientRepository, SensorClientRepository>();
            services.AddScoped<IOutboxSensorDataRepository, OutboxSensorDataRepository>();

            return services;
        }

        private IServiceCollection AddInfluxDb(IConfiguration configuration)
        {
            services.AddSingleton<IInfluxDbService>(sp => new InfluxDbService(configuration));

            return services;
        }

        private IServiceCollection AddUnitOfWork()
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
