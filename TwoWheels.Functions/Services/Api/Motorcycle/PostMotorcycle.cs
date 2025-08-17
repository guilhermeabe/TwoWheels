using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class PostMotorcycle
    {
        private readonly ILogger<PostMotorcycle> _logger;

        public PostMotorcycle(ILogger<PostMotorcycle> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "PostMotorcycle", tags: ["Motorcycle"], Description = "Create a new motorcycle", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostMotorcycle")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "motos")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
