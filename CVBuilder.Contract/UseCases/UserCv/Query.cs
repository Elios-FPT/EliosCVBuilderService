using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Contract.UseCases.UserCv
{
    public static class Query
    {
        public record GetUserCvByIdQuery(
            Guid Id
        ) : IQuery<BaseResponseDto<UserCvDto>>;

        public record GetUserCvsQuery(
            Guid UserId,
            int PageNumber = 1,
            int PageSize = 20
        ) : IQuery<BaseResponseDto<IEnumerable<UserCvDto>>>;
    }
}
