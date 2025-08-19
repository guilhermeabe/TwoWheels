using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwoWheels.Functions.Services.Api.Deliverer;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;

namespace TwoWheels.Test.Integration.Deliverer;

public class PostDelivererIntegrationTests : IntegrationTestBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PostDeliverer> _logger;

    public PostDelivererIntegrationTests()
    {
        _mediator = Scope.ServiceProvider.GetRequiredService<IMediator>();
        _logger = Scope.ServiceProvider.GetRequiredService<ILogger<PostDeliverer>>();
    }

    [Fact]
    public async Task PostDeliverer_Should_Create_Deliverer_Successfully()
    {
        // Arrange
        var command = new CreateDelivererCommand
        {
            Id = "deliverer-test-001",
            Name = "João da Silva",
            Cnpj = "12345678901234",
            CnhNumber = "12345678901",
            CnhTypeString = "AB",
            BirthDate = DateTime.Now.AddYears(-25)
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var delivererInDb = await DbContext.Deliverers.FindAsync(command.Id);
        delivererInDb.Should().NotBeNull();
        delivererInDb!.Name.Should().Be(command.Name);
        delivererInDb.Cnpj.Should().Be(command.Cnpj);
        delivererInDb.CnhNumber.Should().Be(command.CnhNumber);
    }

    [Fact]
    public async Task PostDeliverer_Should_Fail_With_Invalid_Data()
    {
        // Arrange
        var command = new CreateDelivererCommand
        {
            Id = "", // Invalid - empty ID
            Name = "João da Silva",
            Cnpj = "123", // Invalid - wrong length
            CnhNumber = "12345678901",
            CnhTypeString = "AB",
            BirthDate = DateTime.Now.AddYears(-25)
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var delivererInDb = await DbContext.Deliverers.FindAsync(command.Id);
        delivererInDb.Should().BeNull();
    }

    [Fact]
    public async Task PostDeliverer_Should_Fail_With_Duplicate_Cnpj()
    {
        // Arrange
        var firstCommand = new CreateDelivererCommand
        {
            Id = "deliverer-001",
            Name = "João da Silva",
            Cnpj = "12345678901234",
            CnhNumber = "12345678901",
            CnhTypeString = "AB",
            BirthDate = DateTime.Now.AddYears(-25)
        };

        await _mediator.Send(firstCommand);

        var secondCommand = new CreateDelivererCommand
        {
            Id = "deliverer-002",
            Name = "Maria dos Santos",
            Cnpj = "12345678901234",
            CnhNumber = "12345678902",
            CnhTypeString = "A",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        // Act
        var result = await _mediator.Send(secondCommand);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var deliverersCount = DbContext.Deliverers.Count();
        deliverersCount.Should().Be(1);
    }
}