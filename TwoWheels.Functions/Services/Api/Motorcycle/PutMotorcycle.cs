using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class PutMotorcycle
    {
        private readonly ILogger<PostMotorcycle> _logger;

        public PutMotorcycle(ILogger<PostMotorcycle> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "PutMotorcycle", tags: ["Motorcycle"], Description = "Update a motorcycle", Visibility = OpenApiVisibilityType.Important)]
        [Function("PutMotorcycle")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "motos/{id}/placa")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
