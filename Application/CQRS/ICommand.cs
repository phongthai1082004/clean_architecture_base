using MediatR;

namespace Application.CQRS
{
    public interface IBaseCommand { }
    public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand { }
    public interface ICommand : ICommand<Unit> { }
}
