using MediatR;

namespace Application.CQRS
{
    /// <summary>Handles a command and returns its result.</summary>
    /// <typeparam name="TCommand">Command type implementing <see cref="ICommand{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">Result type.</typeparam>
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }

    /// <summary>Handles a command that returns nothing.</summary>
    /// <typeparam name="TCommand">Command type implementing <see cref="ICommand"/>.</typeparam>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit>
        where TCommand : ICommand { }
}
