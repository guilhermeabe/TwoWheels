using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Services.Api.Deliverer
{
    public class PostDeliverer
    {
        private readonly ILogger<PostDeliverer> _logger;

        public PostDeliverer(ILogger<PostDeliverer> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "PostDeliverer", tags: ["Deliverer"], Description = "Create a new deliverer", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostDeliverer")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "entregadores")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
