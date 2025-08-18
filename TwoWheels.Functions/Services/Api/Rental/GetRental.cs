using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using TwoWheels.Functions.Services.Api.Rental.Queries;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace TwoWheels.Functions.Services.Api.Rental
{
    public class GetRental
    {
        private readonly ILogger<GetRental> _logger;
        private readonly IMediator _mediator;

        public GetRental(ILogger<GetRental> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "GetRental", tags: ["Rental"], Description = "Get rental by ID", Visibility = OpenApiVisibilityType.Important)]
        [Function("GetRental")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "locacao/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Getting rental {RentalId}", id);

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                var query = new GetRentalByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess || result.Data == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Locação não encontrada"
                    });
                    return notFoundResponse;
                }

                var rental = result.Data;

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    identificador = rental.Id,
                    valor_diaria = rental.Plan.DailyRate,
                    entregador_id = rental.Deliverer?.Id ?? "",
                    moto_id = rental.Motorcycle?.Id ?? "",
                    data_inicio = rental.StartDate,
                    data_termino = rental.ExpectedEndDate,
                    data_previsao_termino = rental.ExpectedEndDate,
                    data_devolucao = rental.ActualEndDate
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rental {RentalId}", id);
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
