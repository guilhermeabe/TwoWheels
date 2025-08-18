using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;
using TwoWheels.Functions.Services.Storage.Interfaces;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Deliverer.Handlers
{
    public class CreateDelivererHandler : IRequestHandler<CreateDelivererCommand, Result<string>>
    {
        private readonly IDelivererRepository _repository;
        private readonly IStorageService _storageService;
        private readonly string[] _allowedImageFormats = { ".png", ".bmp" };

        public CreateDelivererHandler(IDelivererRepository repository, IStorageService storageService)
        {
            _repository = repository;
            _storageService = storageService;
        }

        public async Task<Result<string>> Handle(CreateDelivererCommand request, CancellationToken cancellationToken)
        {
            var existingByCnpj = await _repository.GetByCnpjAsync(request.Cnpj);
            if (existingByCnpj != null)
            {
                return Result<string>.Failure("CNPJ already exists");
            }

            var existingByCnh = await _repository.GetByCnhNumberAsync(request.CnhNumber);
            if (existingByCnh != null)
            {
                return Result<string>.Failure("CNH number already exists");
            }

            string? imagePath = null;

            if (!string.IsNullOrWhiteSpace(request.CnhImageBase64))
            {
                try
                {
                    imagePath = await _storageService.SaveImageAsync(
                        request.CnhImageBase64,
                        $"cnh_{request.CnhNumber}",
                        _allowedImageFormats);
                }
                catch (ArgumentException)
                {
                    return Result<string>.Failure("Invalid image format. Only PNG and BMP are allowed.");
                }
            }

            var deliverer = new Domains.Entities.Deliverer
            {
                Id = request.Id,
                Name = request.Name,
                Cnpj = request.Cnpj,
                BirthDate = request.BirthDate,
                CnhNumber = request.CnhNumber,
                CnhType = request.CnhType,
                CnhImagePath = imagePath,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(deliverer);

            return Result<string>.Success(deliverer.Id, "Deliverer created successfully");
        }
    }
}