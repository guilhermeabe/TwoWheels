using FluentValidation;
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
        private readonly IMediator _mediator;
        private readonly ILogger<PostCnhDeliverer> _logger;

        public PostCnhDeliverer(IMediator mediator, ILogger<PostCnhDeliverer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [OpenApiOperation(operationId: "PostCnhDeliverer", tags: ["Deliverer"], Description = "Upload CNH image for deliverer", Visibility = OpenApiVisibilityType.Important)]
        [Function("PostCnhDeliverer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "entregadores/{id}/cnh")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Updating CNH image for deliverer {DelivererId}", id);

            UpdateDelivererCnhImageCommand? command;

            try
            {
                command = await JsonSerializer.DeserializeAsync<UpdateDelivererCnhImageCommand>(
                    req.Body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid JSON");
                return await CreateBadRequest(req);
            }

            if (command is null)
                return await CreateBadRequest(req);

            command.Id = id;

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update CNH image for deliverer {DelivererId}: {Error}", id, result);
                return await CreateBadRequest(req);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                mensagem = "Imagem da CNH atualizada com sucesso"
            });

            return response;
        }

        private static async Task<HttpResponseData> CreateBadRequest(HttpRequestData req)
        {
            var res = req.CreateResponse(HttpStatusCode.BadRequest);
            await res.WriteAsJsonAsync(new { mensagem = "Dados inválidos" });
            return res;
        }
    }
}