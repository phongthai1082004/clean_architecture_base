using MediatR;

namespace Application.CQRS
{
    /// <summary>Command that must pass through the idempotency pipeline (requires an Idempotency-Key header).</summary>
    /// <typeparam name="TResponse">Result type returned to the caller.</typeparam>
    public interface IIdempotentCommand<out TResponse> : ICommand<TResponse> { }

    /// <summary>Idempotent command that returns nothing.</summary>
    public interface IIdempotentCommand : IIdempotentCommand<Unit>, ICommand { }
}
