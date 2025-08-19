using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TwoWheels.Functions.Services.Api.Deliverer.Commands;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace TwoWheels.Functions.Services.Api.Deliverer
{
    public class PostDeliverer
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostDeliverer> _logger;

        public PostDeliverer(IMediator mediator, ILogger<PostDeliverer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Function("PostDeliverer")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "entregadores")] HttpRequestData req)
        {
            _logger.LogInformation("Creating deliverer");

            CreateDelivererCommand? command;
            try
            {
                command = await JsonSerializer.DeserializeAsync<CreateDelivererCommand>(
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

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return await CreateBadRequest(req);

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

        private static async Task<HttpResponseData> CreateBadRequest(HttpRequestData req)
        {
            var res = req.CreateResponse(HttpStatusCode.BadRequest);
            await res.WriteAsJsonAsync(new { mensagem = "Dados inválidos" });
            return res;
        }
    }
}