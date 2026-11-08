using MediatR;

namespace Application.CQRS
{
    /// <summary>Marker for every command; lets pipeline behaviors detect write operations.</summary>
    public interface IBaseCommand { }

    /// <summary>Command that changes state and returns a result.</summary>
    /// <typeparam name="TResponse">Result type returned to the caller.</typeparam>
    public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand { }

    /// <summary>Command that changes state and returns nothing (<see cref="Unit"/>).</summary>
    public interface ICommand : ICommand<Unit> { }
}
