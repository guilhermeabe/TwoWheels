using TwoWheels.Functions.Services.Api.Deliverer.Commands;

namespace TwoWheels.Test.Services.Api.Deliverer.Commands;

public class CreateDelivererCommandTests
{
    [Fact]
    public void CreateDelivererCommand_Should_Create_Valid_Instance()
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

        // Act & Assert
        command.Id.Should().Be("test-id");
        command.Name.Should().Be("John Doe");
        command.Cnpj.Should().Be("12345678901234");
        command.BirthDate.Should().Be(new DateTime(1990, 1, 1));
        command.CnhNumber.Should().Be("12345678901");
        command.CnhTypeString.Should().Be("A");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreateDelivererCommand_Should_Handle_Invalid_Strings(string invalidValue)
    {
        // Arrange
        var command = new CreateDelivererCommand
        {
            Id = invalidValue,
            Name = invalidValue,
            Cnpj = invalidValue,
            CnhNumber = invalidValue,
            CnhTypeString = invalidValue
        };

        // Act & Assert
        command.Id.Should().Be(invalidValue);
        command.Name.Should().Be(invalidValue);
        command.Cnpj.Should().Be(invalidValue);
        command.CnhNumber.Should().Be(invalidValue);
        command.CnhTypeString.Should().Be(invalidValue);
    }
}