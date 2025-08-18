using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using TwoWheels.Functions.Services.Api.Motorcycle.Commands;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class DeleteMotorcycle
    {
        private readonly ILogger<PostMotorcycle> _logger;
        private readonly IMediator _mediator;

        public DeleteMotorcycle(ILogger<PostMotorcycle> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "DeleteMotorcycle", tags: ["Motorcycle"], Description = "Delete a motorcycle", Visibility = OpenApiVisibilityType.Important)]
        [Function("DeleteMotorcycle")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "motos/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Deleting motorcycle {MotorcycleId}", id);

            try
            {
                var command = new DeleteMotorcycleCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await errorResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return errorResponse;
                }
                var response = req.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting motorcycle");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteAsJsonAsync(new
                {
                    mensagem = "Dados inválidos"
                });
                return errorResponse;
            }
        }
    }
}
