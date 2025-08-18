using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace TwoWheels.Functions.Services.Api.Deliverer
{
    public class PostDeliverer
    {
        private readonly ILogger<PostDeliverer> _logger;
        private readonly IMediator _mediator;

        public PostDeliverer(ILogger<PostDeliverer> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "PostDeliverer", tags: ["Deliverer"], Description = "Create a new deliverer", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostDeliverer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "entregadores")] HttpRequestData req)
        {
            _logger.LogInformation("Creating deliverer");

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

                var command = JsonSerializer.Deserialize<CreateDelivererCommand>(body);

                if (command == null ||
                    string.IsNullOrWhiteSpace(command.Id) ||
                    string.IsNullOrWhiteSpace(command.Name) ||
                    string.IsNullOrWhiteSpace(command.Cnpj) ||
                    string.IsNullOrWhiteSpace(command.CnhNumber) ||
                    string.IsNullOrWhiteSpace(command.CnhTypeString) ||
                    command.BirthDate == default)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                if (command.Cnpj.Length != 14 || !command.Cnpj.All(char.IsDigit))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                if (command.CnhNumber.Length != 11 || !command.CnhNumber.All(char.IsDigit))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new
                    {
                        mensagem = "Dados inválidos"
                    });
                    return badResponse;
                }

                if (!IsValidCnhType(command.CnhTypeString))
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
                    nome = command.Name,
                    cnpj = command.Cnpj,
                    data_nascimento = command.BirthDate,
                    numero_cnh = command.CnhNumber,
                    tipo_cnh = command.CnhTypeString
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating deliverer");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteAsJsonAsync(new
                {
                    mensagem = "Dados inválidos"
                });
                return errorResponse;
            }
        }

        private static bool IsValidCnhType(string cnhType)
        {
            var validTypes = new[] { "A", "B", "AB", "A+B" };
            return validTypes.Contains(cnhType.ToUpperInvariant());
        }
    }
}