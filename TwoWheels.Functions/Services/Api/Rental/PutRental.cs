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
    public class PutRental
    {
        private readonly ILogger<PutRental> _logger;
        private readonly IMediator _mediator;

        public PutRental(ILogger<PutRental> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "PutRental", tags: ["Rental"], Description = "Update rental return date", Visibility = OpenApiVisibilityType.Important)]
        [Function("PutRental")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "locacao/{id}/devolucao")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Updating rental return date for {RentalId}", id);

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

                var command = JsonSerializer.Deserialize<UpdateRentalReturnCommand>(body);

                if (command == null || command.ReturnDate == default)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                if (string.IsNullOrWhiteSpace(id))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                if (command.ReturnDate.Date < DateTime.UtcNow.Date)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                command.Id = id;
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

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    mensagem = "Data de devolução informada com sucesso",
                    valor = result.Data
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rental return date for {RentalId}", id);
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