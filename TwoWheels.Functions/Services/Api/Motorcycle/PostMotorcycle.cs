using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TwoWheels.Functions.Services.Api.Motorcycle.Commands;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace TwoWheels.Functions.Services.Api.Motorcycle
{
    public class PostMotorcycle
    {
        private readonly ILogger<PostMotorcycle> _logger;
        private readonly IMediator _mediator;

        public PostMotorcycle(ILogger<PostMotorcycle> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "PostMotorcycle", tags: ["Motorcycle"], Description = "Create a new motorcycle", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostMotorcycle")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "motos")] HttpRequestData req)
        {
            _logger.LogInformation("Creating motorcycle");

            try
            {
                var body = await req.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                var command = JsonSerializer.Deserialize<CreateMotorcycleCommand>(body);
                if (command == null ||
                    string.IsNullOrWhiteSpace(command.Id) ||
                    string.IsNullOrWhiteSpace(command.Model) ||
                    string.IsNullOrWhiteSpace(command.LicensePlate) ||
                    command.Year <= 0)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await errorResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return errorResponse;
                }

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(new
                {
                    identificador = command.Id,
                    ano = command.Year,
                    modelo = command.Model,
                    placa = command.LicensePlate
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating motorcycle");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteAsJsonAsync(new
                {
                    mensagem = "Dados inválidos"
                });
                return errorResponse;
            }
        }
    }
}