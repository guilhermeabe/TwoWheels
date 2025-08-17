using MediatR;

namespace TwoWheels.Functions.Shared.Mediator
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }

    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
