using MediatR;
using CVBuilder.Contract.Shared;

namespace CVBuilder.Contract.Message
{
    public interface IQuery<TResponse> : IRequest<TResponse>;
}
