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
    public class GetMotorcycles
    {
        private readonly ILogger<PostMotorcycle> _logger;
        private readonly IMediator _mediator;

        public GetMotorcycles(ILogger<PostMotorcycle> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "GetMotorcycles", tags: ["Motorcycle"], Description = "Get by param or list all Motorcycles", Visibility = OpenApiVisibilityType.Important)]
        [Function("GetMotorcycles")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "motos")] HttpRequestData req)
        {
            _logger.LogInformation("Getting motorcycles");

            try
            {
                var query = new GetMotorcyclesQuery();

                var result = await _mediator.Send(query);

                var motorcyclesResponse = result.Data?.Select(m => new
                {
                    identificador = m.Id,
                    ano = m.Year,
                    modelo = m.Model,
                    placa = m.LicensePlate
                }).ToList();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(motorcyclesResponse);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting motorcycles");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Internal server error");
                return errorResponse;
            }
        }
    }
}
