using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class DeleteMotorcycle
    {
        private readonly ILogger<PostMotorcycle> _logger;

        public DeleteMotorcycle(ILogger<PostMotorcycle> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "DeleteMotorcycle", tags: ["Motorcycle"], Description = "Delete a motorcycle", Visibility = OpenApiVisibilityType.Important)]
        [Function("DeleteMotorcycle")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "motos/{id}")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
