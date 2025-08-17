using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Services.Api.Deliverer
{
    public class PostCnhDeliverer
    {
        private readonly ILogger<PostCnhDeliverer> _logger;

        public PostCnhDeliverer(ILogger<PostCnhDeliverer> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "PostCnhDeliverer", tags: ["Deliverer"], Description = "Create a new cnh deliverer", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostCnhDeliverer")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "entregadores/{id}/cnh")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
