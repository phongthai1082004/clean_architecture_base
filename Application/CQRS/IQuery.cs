using MediatR;

namespace Application.CQRS
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
