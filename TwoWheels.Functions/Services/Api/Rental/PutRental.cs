using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Services.Api.Rental
{
    public class PutRental
    {
        private readonly ILogger<PutRental> _logger;

        public PutRental(ILogger<PutRental> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "PutRental", tags: ["Rental"], Description = "Update a rental", Visibility = OpenApiVisibilityType.Important)]
        [Function("PutRental")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "locacao/{id}/devolucao")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
