using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwoWheels.Functions.Infra.Repositories.Data;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Events.Interfaces;
using TwoWheels.Functions.Services.Events.Motorcycle.Models;
using TwoWheels.Functions.Services.Storage.Interfaces;
using TwoWheels.Functions.Infra.Repositories;
using TwoWheels.Functions.Services.Api.Deliverer.Validators;
using TwoWheels.Functions.Shared.Behaviors;

namespace TwoWheels.Test.Integration;

public abstract class IntegrationTestBase : IDisposable
{
    protected readonly IHost Host;
    protected readonly IServiceScope Scope;
    protected readonly AppDbContext DbContext;
    private bool _disposed;

    protected IntegrationTestBase()
    {
        Host = CreateHostBuilder().Build();
        Scope = Host.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();

        DbContext.Database.EnsureCreated();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TwoWheels.Functions.Services.Api.Deliverer.Commands.CreateDelivererCommand).Assembly));

                services.AddScoped<IEventPublisher, MockEventPublisher>();
                services.AddScoped<IStorageService, MockStorageService>();

                services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
                services.AddScoped<IDelivererRepository, DelivererRepository>();
                services.AddScoped<IRentalRepository, RentalRepository>();

                services.AddValidatorsFromAssemblyContaining<CreateDelivererCommandValidator>();
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

                services.AddLogging(builder => builder.AddConsole());
            });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DbContext?.Dispose();
                Scope?.Dispose();
                Host?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public class MockEventPublisher : IEventPublisher
{
    public Task PublishMotorcycleCreatedAsync(MotorcycleCreatedEvent motorcycleEvent)
    {
        return Task.CompletedTask;
    }
}

public class MockStorageService : IStorageService
{
    public Task<string> SaveImageAsync(string base64Image, string fileName, string[] allowedExtensions)
    {
        return Task.FromResult($"mock-url/{fileName}");
    }

    public bool DeleteImageAsync(string filePath)
    {
        return true;
    }

    public bool ValidateImageFormat(string base64Image, string[] allowedExtensions)
    {
        return true;
    }
}