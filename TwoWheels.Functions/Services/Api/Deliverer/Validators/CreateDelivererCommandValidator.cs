using FluentValidation;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;

namespace TwoWheels.Functions.Services.Api.Deliverer.Validators
{
    public class CreateDelivererCommandValidator : AbstractValidator<CreateDelivererCommand>
    {
        public CreateDelivererCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Identificador é obrigatório");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome é obrigatório");

            RuleFor(x => x.Cnpj)
                .NotEmpty()
                .WithMessage("CNPJ é obrigatório")
                .Length(14)
                .WithMessage("CNPJ deve ter 14 dígitos")
                .Must(BeNumericOnly)
                .WithMessage("CNPJ deve conter apenas números");

            RuleFor(x => x.CnhNumber)
                .NotEmpty()
                .WithMessage("Número da CNH é obrigatório")
                .Length(11)
                .WithMessage("CNH deve ter 11 dígitos")
                .Must(BeNumericOnly)
                .WithMessage("CNH deve conter apenas números");

            RuleFor(x => x.CnhTypeString)
                .NotEmpty()
                .WithMessage("Tipo da CNH é obrigatório")
                .Must(BeValidCnhType)
                .WithMessage("Tipo da CNH deve ser A, B, AB ou A+B");

            RuleFor(x => x.BirthDate)
                .NotEqual(default(DateTime))
                .WithMessage("Data de nascimento é obrigatória")
                .LessThan(DateTime.Now.AddYears(-18))
                .WithMessage("Entregador deve ser maior de idade");
        }

        private static bool BeNumericOnly(string value)
        {
            return !string.IsNullOrEmpty(value) && value.All(char.IsDigit);
        }

        private static bool BeValidCnhType(string cnhType)
        {
            if (string.IsNullOrWhiteSpace(cnhType))
                return false;

            var validTypes = new[] { "A", "B", "AB", "A+B" };
            return validTypes.Contains(cnhType.ToUpperInvariant());
        }
    }
}