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
    public static class Command
    {
        public record CreateUserCvCommand(
            Guid UserId,
            Guid TemplateId,
            string Title
        ) : ICommand<BaseResponseDto<UserCvDto>>;

        public record UpdateUserCvCommand(
            Guid Id,
            string Title
        ) : ICommand<BaseResponseDto<UserCvDto>>;

        public record DeleteUserCvCommand(
            Guid Id
        ) : ICommand<BaseResponseDto<bool>>;
    }
}
