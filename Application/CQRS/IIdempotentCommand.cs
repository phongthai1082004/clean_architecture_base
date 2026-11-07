using MediatR;

namespace Application.CQRS
{
    public interface IIdempotentCommand<out TResponse> : ICommand<TResponse> { }
    public interface IIdempotentCommand : IIdempotentCommand<Unit>, ICommand { }
}
