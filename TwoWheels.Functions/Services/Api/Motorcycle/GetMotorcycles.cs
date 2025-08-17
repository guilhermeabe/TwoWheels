using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class GetMotorcycles
    {
        private readonly ILogger<PostMotorcycle> _logger;

        public GetMotorcycles(ILogger<PostMotorcycle> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "GetMotorcycles", tags: ["Motorcycles"], Description = "Get by param or list all Motorcycles", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Id/license plate of motorcycle")]
        [Function("GetMotorcycles")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "motos")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
