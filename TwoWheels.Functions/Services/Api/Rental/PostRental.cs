using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Services.Api.Rental
{
    public class PostRental
    {
        private readonly ILogger<PostRental> _logger;

        public PostRental(ILogger<PostRental> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "PostRental", tags: ["Rental"], Description = "Create a new rental", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostRental")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "locacao")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
