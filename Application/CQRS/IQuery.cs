using MediatR;

namespace Application.CQRS
{
    /// <summary>Read-only operation that returns a result.</summary>
    /// <typeparam name="TResponse">Result type returned to the caller.</typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
