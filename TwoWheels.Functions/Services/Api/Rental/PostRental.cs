using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TwoWheels.Functions.Services.Api.Rental.Commands;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace TwoWheels.Functions.Services.Api.Rental
{
    public class PostRental
    {
        private readonly ILogger<PostRental> _logger;
        private readonly IMediator _mediator;

        public PostRental(ILogger<PostRental> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "PostRental", tags: ["Rental"], Description = "Create a new rental", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostRental")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "locacao")] HttpRequestData req)
        {
            _logger.LogInformation("Creating rental");

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

                var command = JsonSerializer.Deserialize<CreateRentalCommand>(body);

                if (command == null ||
                    string.IsNullOrWhiteSpace(command.DelivererId) ||
                    string.IsNullOrWhiteSpace(command.MotorcycleId) ||
                    command.StartDate == default ||
                    command.EndDate == default ||
                    command.ExpectedEndDate == default ||
                    command.PlanDays <= 0)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                var validPlans = new[] { 7, 15, 30, 45, 50 };
                if (!validPlans.Contains(command.PlanDays))
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
                    identificador = result.Data,
                    entregador_id = command.DelivererId,
                    moto_id = command.MotorcycleId,
                    data_inicio = command.StartDate,
                    data_termino = command.EndDate,
                    data_previsao_termino = command.ExpectedEndDate,
                    plano = command.PlanDays
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental");
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