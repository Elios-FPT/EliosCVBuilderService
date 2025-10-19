using MediatR;
using CVBuilder.Contract.Shared;

namespace CVBuilder.Contract.Message
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>;
}
