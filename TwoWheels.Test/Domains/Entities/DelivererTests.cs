using TwoWheels.Functions.Domains.Entities;
using TwoWheels.Functions.Domains.Enuns;

namespace TwoWheels.Test.Domains.Entities;

public class DelivererTests
{
    [Fact]
    public void Deliverer_Should_Create_Valid_Instance()
    {
        // Arrange
        var id = "test-id";
        var name = "John Doe";
        var cnpj = "12345678901234";
        var birthDate = new DateTime(1990, 1, 1);
        var cnhNumber = "12345678901";
        var cnhType = CnhType.A;

        // Act
        var deliverer = new Deliverer
        {
            Id = id,
            Name = name,
            Cnpj = cnpj,
            BirthDate = birthDate,
            CnhNumber = cnhNumber,
            CnhType = cnhType
        };

        // Assert
        deliverer.Id.Should().Be(id);
        deliverer.Name.Should().Be(name);
        deliverer.Cnpj.Should().Be(cnpj);
        deliverer.BirthDate.Should().Be(birthDate);
        deliverer.CnhNumber.Should().Be(cnhNumber);
        deliverer.CnhType.Should().Be(cnhType);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("AB")]
    [InlineData("A+B")]
    public void Deliverer_Should_Accept_Valid_CnhTypes(string cnhTypeString)
    {
        // Arrange & Act
        var isValid = Enum.TryParse<CnhType>(cnhTypeString.Replace("+", ""), out var cnhType);

        // Assert
        isValid.Should().BeTrue();
    }
}