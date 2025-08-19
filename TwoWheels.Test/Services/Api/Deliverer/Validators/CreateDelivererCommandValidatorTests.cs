using FluentValidation.TestHelper;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;
using TwoWheels.Functions.Services.Api.Deliverer.Validators;

namespace TwoWheels.Test.Services.Api.Deliverer.Validators
{
    public class CreateDelivererCommandValidatorTests
    {
        private readonly CreateDelivererCommandValidator _validator = new();

        private CreateDelivererCommand MakeValid() => new()
        {
            Id = "deliverer-001",
            Name = "João Silva",
            Cnpj = "12345678901234",
            CnhNumber = "12345678901",
            CnhTypeString = "AB",
            BirthDate = DateTime.Now.AddYears(-25)
        };

        [Fact]
        public void Valid_Command_Should_Pass()
        {
            var cmd = MakeValid();
            var result = _validator.TestValidate(cmd);
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Id_Empty_Should_Fail(string? id)
        {
            var cmd = MakeValid();
            cmd.Id = id!;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.Id)
                  .WithErrorMessage("Identificador é obrigatório");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Name_Empty_Should_Fail(string? name)
        {
            var cmd = MakeValid();
            cmd.Name = name!;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.Name)
                  .WithErrorMessage("Nome é obrigatório");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Cnpj_Empty_Should_Fail(string? cnpj)
        {
            var cmd = MakeValid();
            cmd.Cnpj = cnpj!;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.Cnpj)
                  .WithErrorMessage("CNPJ é obrigatório");
        }

        [Theory]
        [InlineData("1234567890123")]      // 13
        [InlineData("123456789012345")]    // 15
        public void Cnpj_Wrong_Length_Should_Fail(string cnpj)
        {
            var cmd = MakeValid();
            cmd.Cnpj = cnpj;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.Cnpj)
                  .WithErrorMessage("CNPJ deve ter 14 dígitos");
        }

        [Theory]
        [InlineData("1234567890123A")]
        [InlineData("1234567890123.")]
        public void Cnpj_NonNumeric_Should_Fail(string cnpj)
        {
            var cmd = MakeValid();
            cmd.Cnpj = cnpj;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.Cnpj)
                  .WithErrorMessage("CNPJ deve conter apenas números");
        }

        [Theory]
        [InlineData("1234567890")]      // 10
        [InlineData("123456789012")]    // 12
        public void CnhNumber_Wrong_Length_Should_Fail(string cnh)
        {
            var cmd = MakeValid();
            cmd.CnhNumber = cnh;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.CnhNumber)
                  .WithErrorMessage("CNH deve ter 11 dígitos");
        }

        [Theory]
        [InlineData("1234567890A")]
        [InlineData("1234567890-1")]
        public void CnhNumber_NonNumeric_Should_Fail(string cnh)
        {
            var cmd = MakeValid();
            cmd.CnhNumber = cnh;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.CnhNumber)
                  .WithErrorMessage("CNH deve conter apenas números");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("X")]
        [InlineData("ABC")]
        public void CnhType_Invalid_Should_Fail(string? type)
        {
            var cmd = MakeValid();
            cmd.CnhTypeString = type!;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.CnhTypeString)
                  .WithErrorMessage("Tipo da CNH deve ser A, B, AB ou A+B");
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("AB")]
        [InlineData("A+B")]
        [InlineData("a")]
        [InlineData("ab")]
        public void CnhType_Valid_Should_Pass(string type)
        {
            var cmd = MakeValid();
            cmd.CnhTypeString = type;
            var result = _validator.TestValidate(cmd);
            result.ShouldNotHaveValidationErrorFor(c => c.CnhTypeString);
        }

        [Fact]
        public void BirthDate_Default_Should_Fail()
        {
            var cmd = MakeValid();
            cmd.BirthDate = default;
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.BirthDate)
                  .WithErrorMessage("Data de nascimento é obrigatória");
        }

        [Fact]
        public void BirthDate_Under18_Should_Fail()
        {
            var cmd = MakeValid();
            cmd.BirthDate = DateTime.Now.AddYears(-17).AddDays(1);
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.BirthDate)
                  .WithErrorMessage("Entregador deve ser maior de idade");
        }

        [Fact]
        public void BirthDate_Exactly18YearsAgo_Should_Pass()
        {
            var cmd = MakeValid();
            cmd.BirthDate = DateTime.Now.AddYears(-18).AddDays(-1);
            var result = _validator.TestValidate(cmd);
            result.ShouldNotHaveValidationErrorFor(c => c.BirthDate);
        }
    }
}
