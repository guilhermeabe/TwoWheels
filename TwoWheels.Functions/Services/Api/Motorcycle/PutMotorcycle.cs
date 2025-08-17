using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TwoWheels.Functions.Services.Api.Motorcycle.Commands;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class PutMotorcycle
    {
        private readonly ILogger<PostMotorcycle> _logger;
        private readonly IMediator _mediator;

        public PutMotorcycle(ILogger<PostMotorcycle> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "PutMotorcycle", tags: ["Motorcycle"], Description = "Update a motorcycle", Visibility = OpenApiVisibilityType.Important)]
        [Function("PutMotorcycle")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "motos/{id}/placa")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Updating motorcycle {MotorcycleId}", id);

            try
            {
                var body = await req.ReadAsStringAsync();
                var command = JsonSerializer.Deserialize<UpdateMotorcycleCommand>(body ?? "{}");

                if (command == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid request body");
                    return badResponse;
                }

                command.Id = id;
                var result = await _mediator.Send(command);

                var response = req.CreateResponse(result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
                await response.WriteAsJsonAsync(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    errors = result.Errors
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating motorcycle");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Internal server error");
                return errorResponse;
            }
        }
    }
}
