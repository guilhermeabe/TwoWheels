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
    public class PostMotorcycle
    {
        private readonly ILogger<PostMotorcycle> _logger;
        private readonly IMediator _mediator;

        public PostMotorcycle(ILogger<PostMotorcycle> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "PostMotorcycle", tags: ["Motorcycle"], Description = "Create a new motorcycle", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostMotorcycle")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "motos")] HttpRequestData req)
        {
            _logger.LogInformation("Creating motorcycle");

            try
            {
                var body = await req.ReadAsStringAsync();
                var command = JsonSerializer.Deserialize<CreateMotorcycleCommand>(body ?? "{}");

                if (command == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid request body");
                    return badResponse;
                }

                var result = await _mediator.Send(command);

                var response = req.CreateResponse(result.IsSuccess ? HttpStatusCode.Created : HttpStatusCode.BadRequest);
                await response.WriteAsJsonAsync(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    data = result.Data,
                    errors = result.Errors
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating motorcycle");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Internal server error");
                return errorResponse;
            }
        }
    }
}
