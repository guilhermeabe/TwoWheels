using FluentValidation;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;

namespace TwoWheels.Functions.Services.Api.Deliverer.Validators
{
    public class UpdateDelivererCnhImageCommandValidator : AbstractValidator<UpdateDelivererCnhImageCommand>
    {
        public UpdateDelivererCnhImageCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Identificador é obrigatório");

            RuleFor(x => x.CnhImageBase64)
                .NotEmpty()
                .WithMessage("Imagem da CNH é obrigatória")
                .Must(BeValidBase64)
                .WithMessage("Imagem da CNH deve estar em formato Base64 válido")
                .Must(HaveMinimumSize)
                .WithMessage("Imagem da CNH deve ter tamanho mínimo válido");
        }

        private static bool BeValidBase64(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            try
            {
                var base64Data = base64String;
                if (base64String.Contains(','))
                    base64Data = base64String.Split(',')[1];

                Convert.FromBase64String(base64Data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool HaveMinimumSize(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            try
            {
                var base64Data = base64String;
                if (base64String.Contains(','))
                    base64Data = base64String.Split(',')[1];

                return base64Data.Length > 10;
            }
            catch
            {
                return false;
            }
        }
    }
}