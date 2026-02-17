using AgroSolutions.Ingest.Application;
using AgroSolutions.Ingest.Application.Behaviors;
using AgroSolutions.Ingest.Application.Commands.SaveSensorData;
using AgroSolutions.Ingest.Application.Notifications;
using AgroSolutions.Ingest.Domain.Notifications;
using AgroSolutions.Ingest.Infrastructure.Subscribers;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AgroSolutions.Ingest.Application;

public static class ApplicationModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services
                .AddSubscribers()
                .AddMediatR()
                .AddFluentValidation()
                .AddNotification();

            return services;
        }

        private IServiceCollection AddSubscribers()
        {
            services.AddHostedService<ReceivedSensorDataSubscriber>();

            return services;
        }

        private IServiceCollection AddMediatR()
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<SaveSensorDataCommandHandler>());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

        private IServiceCollection AddFluentValidation()
        {
            services
                .AddFluentValidationAutoValidation(o => o.DisableDataAnnotationsValidation = true)
                .AddValidatorsFromAssemblyContaining<SaveSensorDataCommandValidator>();

            return services;
        }

        private IServiceCollection AddNotification()
        {
            services.AddScoped<INotificationContext, NotificationContext>();

            return services;
        }
    }
}
