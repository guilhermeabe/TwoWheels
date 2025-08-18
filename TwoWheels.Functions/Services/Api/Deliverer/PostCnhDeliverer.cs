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
    public class PostCnhDeliverer
    {
        private readonly ILogger<PostCnhDeliverer> _logger;
        private readonly IMediator _mediator;

        public PostCnhDeliverer(ILogger<PostCnhDeliverer> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [OpenApiOperation(operationId: "PostCnhDeliverer", tags: ["Deliverer"], Description = "Upload CNH image for deliverer", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostCnhDeliverer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "entregadores/{id}/cnh")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Updating CNH image for deliverer {DelivererId}", id);

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

                var command = JsonSerializer.Deserialize<UpdateDelivererCnhImageCommand>(body);

                if (command == null || string.IsNullOrWhiteSpace(command.CnhImageBase64))
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

                if (!IsValidBase64(command.CnhImageBase64))
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
                    mensagem = "Imagem da CNH atualizada com sucesso"
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CNH image for deliverer {DelivererId}", id);
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteAsJsonAsync(new
                {
                    mensagem = "Dados inválidos"
                });
                return errorResponse;
            }
        }

        private static bool IsValidBase64(string base64String)
        {
            try
            {
                var base64Data = base64String;
                if (base64String.Contains(','))
                    base64Data = base64String.Split(',')[1];

                Convert.FromBase64String(base64Data);

                return base64Data.Length > 10;
            }
            catch
            {
                return false;
            }
        }
    }
}