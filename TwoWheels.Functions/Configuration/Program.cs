using FluentValidation;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwoWheels.Functions.Infra.Repositories;
using TwoWheels.Functions.Infra.Repositories.Data;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Deliverer.Validators;
using TwoWheels.Functions.Services.Events;
using TwoWheels.Functions.Services.Events.Interfaces;
using TwoWheels.Functions.Services.Storage;
using TwoWheels.Functions.Services.Storage.Interfaces;
using TwoWheels.Functions.Shared.Behaviors;
using TwoWheels.Functions.Shared.Decorator;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("DefaultConnection")));

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        // Repositories
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IDelivererRepository, DelivererRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IMongoDbService, MongoDbService>();

        // Storage
        services.AddScoped<IStorageService, LocalStorageService>();

        // Decorators
        services.Decorate(typeof(IRequestHandler<,>), typeof(LoggingDecorator<,>));

        //Events
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        //Validation
        services.AddValidatorsFromAssemblyContaining<CreateDelivererCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateDelivererCnhImageCommandValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    })
    .Build();

host.Run();