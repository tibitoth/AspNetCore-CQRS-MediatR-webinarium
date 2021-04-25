using MediatR;

namespace CqrsMediator.Demo.Bll.Mediator
{
    public interface ICommand<TResult> : IRequest<TResult>
    {
    }
}
