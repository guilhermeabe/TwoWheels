using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Web;
using TwoWheels.Functions.Services.Api.Motorcycle.Queries;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class GetMotorcycleById
    {
        private readonly ILogger<GetMotorcycleById> _logger;
        private readonly IMediator _mediator;

        public GetMotorcycleById(ILogger<GetMotorcycleById> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "GetMotorcycleById", tags: ["Motorcycle"], Description = "Get motorcycle by id", Visibility = OpenApiVisibilityType.Important)]
        [Function("GetMotorcycleById")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "motos/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Getting motorcycle with ID: {MotorcycleId}", id);

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                var query = new GetMotorcycleByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess || result.Data == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return notFoundResponse;
                }

                var motorcycle = result.Data;
                var motorcycleResponse = new
                {
                    identificador = motorcycle.Id,
                    ano = motorcycle.Year,
                    modelo = motorcycle.Model,
                    placa = motorcycle.LicensePlate
                };

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(motorcycleResponse);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting motorcycle with ID: {MotorcycleId}", id);
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