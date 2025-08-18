using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;
using TwoWheels.Functions.Services.Storage.Interfaces;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Deliverer.Handlers
{
    public class UpdateDelivererCnhImageHandler : IRequestHandler<UpdateDelivererCnhImageCommand, Result>
    {
        private readonly IDelivererRepository _repository;
        private readonly IStorageService _storageService;
        private readonly string[] _allowedImageFormats = { ".png", ".bmp" };

        public UpdateDelivererCnhImageHandler(IDelivererRepository repository, IStorageService storageService)
        {
            _repository = repository;
            _storageService = storageService;
        }

        public async Task<Result> Handle(UpdateDelivererCnhImageCommand request, CancellationToken cancellationToken)
        {
            var deliverer = await _repository.GetByIdAsync(request.Id);
            if (deliverer == null)
            {
                return Result.Failure("Deliverer not found");
            }

            try
            {
                var newImagePath = await _storageService.SaveImageAsync(
                    request.CnhImageBase64,
                    $"cnh_{deliverer.CnhNumber}",
                    _allowedImageFormats);

                if (!string.IsNullOrWhiteSpace(deliverer.CnhImagePath))
                    _storageService.DeleteImageAsync(deliverer.CnhImagePath);

                deliverer.CnhImagePath = newImagePath;
                deliverer.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(deliverer);

                return Result.Success("CNH image updated successfully");
            }
            catch (ArgumentException)
            {
                return Result.Failure("Invalid image format. Only PNG and BMP are allowed.");
            }
        }
    }
}