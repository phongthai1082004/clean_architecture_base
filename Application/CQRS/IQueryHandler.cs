using MediatR;

namespace Application.CQRS
{
    /// <summary>Handles a query and returns its result.</summary>
    /// <typeparam name="TQuery">Query type implementing <see cref="IQuery{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">Result type returned to the caller.</typeparam>
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse> { }
}
