using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;
using TwoWheels.Functions.Services.Api.Deliverer.Handlers;
using TwoWheels.Functions.Services.Storage.Interfaces;

namespace TwoWheels.Test.Services.Api.Deliverer.Handlers;

public class CreateDelivererHandlerTests
{
    private readonly Mock<IDelivererRepository> _mockRepository;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly CreateDelivererHandler _handler;

    public CreateDelivererHandlerTests()
    {
        _mockRepository = new Mock<IDelivererRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _handler = new CreateDelivererHandler(_mockRepository.Object, _mockStorageService.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Deliverer_Successfully()
    {
        // Arrange
        var command = new CreateDelivererCommand
        {
            Id = "test-id",
            Name = "John Doe",
            Cnpj = "12345678901234",
            BirthDate = new DateTime(1990, 1, 1),
            CnhNumber = "12345678901",
            CnhTypeString = "A"
        };

        _mockRepository.Setup(x => x.GetByCnpjAsync(command.Cnpj))
            .ReturnsAsync((Functions.Domains.Entities.Deliverer?)null);
        _mockRepository.Setup(x => x.GetByCnhNumberAsync(command.CnhNumber))
            .ReturnsAsync((Functions.Domains.Entities.Deliverer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Functions.Domains.Entities.Deliverer>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Cnpj_Already_Exists()
    {
        // Arrange
        var command = new CreateDelivererCommand
        {
            Id = "test-id",
            Name = "John Doe",
            Cnpj = "12345678901234",
            BirthDate = new DateTime(1990, 1, 1),
            CnhNumber = "12345678901",
            CnhTypeString = "A"
        };

        var existingDeliverer = new Functions.Domains.Entities.Deliverer { Id = "existing-id", Cnpj = command.Cnpj };
        _mockRepository.Setup(x => x.GetByCnpjAsync(command.Cnpj))
            .ReturnsAsync(existingDeliverer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Functions.Domains.Entities.Deliverer>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_CnhNumber_Already_Exists()
    {
        // Arrange
        var command = new CreateDelivererCommand
        {
            Id = "test-id",
            Name = "John Doe",
            Cnpj = "12345678901234",
            BirthDate = new DateTime(1990, 1, 1),
            CnhNumber = "12345678901",
            CnhTypeString = "A"
        };

        var existingDeliverer = new Functions.Domains.Entities.Deliverer { Id = "existing-id", CnhNumber = command.CnhNumber };
        _mockRepository.Setup(x => x.GetByCnpjAsync(command.Cnpj))
            .ReturnsAsync((Functions.Domains.Entities.Deliverer?)null);
        _mockRepository.Setup(x => x.GetByCnhNumberAsync(command.CnhNumber))
            .ReturnsAsync(existingDeliverer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Functions.Domains.Entities.Deliverer>()), Times.Never);
    }
}