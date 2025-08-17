using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Web;
using TwoWheels.Functions.Services.Api.Motorcycle.Queries;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class GetMotorcycles
    {
        private readonly ILogger<PostMotorcycle> _logger;
        private readonly IMediator _mediator;

        public GetMotorcycles(ILogger<PostMotorcycle> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "GetMotorcycles", tags: ["Motorcycles"], Description = "Get by param or list all Motorcycles", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Id/license plate of motorcycle")]
        [Function("GetMotorcycles")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "motos")] HttpRequestData req)
        {
            _logger.LogInformation("Getting motorcycles");

            try
            {
                var query = new GetMotorcyclesQuery();

                var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
                query.LicensePlate = queryParams["id"];

                var result = await _mediator.Send(query);

                var response = req.CreateResponse(HttpStatusCode.OK);
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
                _logger.LogError(ex, "Error getting motorcycles");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Internal server error");
                return errorResponse;
            }
        }
    }
}
