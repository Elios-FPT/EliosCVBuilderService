using MediatR;
using CVBuilder.Contract.Shared;

namespace CVBuilder.Contract.Message
{
    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>;
}
