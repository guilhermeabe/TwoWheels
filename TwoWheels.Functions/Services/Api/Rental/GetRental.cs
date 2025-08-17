using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace TwoWheels.Functions.Services.Api.Rental
{
    public class GetRental
    {
        private readonly ILogger<GetRental> _logger;

        public GetRental(ILogger<GetRental> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "GetRental", tags: ["Rental"], Description = "Get by param", Visibility = OpenApiVisibilityType.Important)]
        [Function("GetRental")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "locacao/{id}")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
