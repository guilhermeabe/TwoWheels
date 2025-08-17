using MediatR;
using Microsoft.Extensions.Logging;

namespace TwoWheels.Functions.Shared.Decorator
{
    public class LoggingDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly ILogger<LoggingDecorator<TRequest, TResponse>> _logger;

        public LoggingDecorator(IRequestHandler<TRequest, TResponse> handler, ILogger<LoggingDecorator<TRequest, TResponse>> logger)
        {
            _handler = handler;
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("Executing request: {RequestName}", requestName);

            try
            {
                var result = await _handler.Handle(request, cancellationToken);
                _logger.LogInformation("Request {RequestName} executed successfully", requestName);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing request: {RequestName}", requestName);
                throw;
            }
        }
    }
}
