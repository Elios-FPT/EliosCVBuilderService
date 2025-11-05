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
        public record UpdateUserCvCommand(
            Guid Id,
            string Title
        ) : ICommand<BaseResponseDto<UserCvDto>>;

        public record DeleteUserCvCommand(
            Guid Id
        ) : ICommand<BaseResponseDto<bool>>;

        public record CreateUserCvCommand(
            Guid Id,
            string Body
        ) : ICommand<BaseResponseDto<UserCvDto>>;

        public record UpdateUserCvCommandV2(
            Guid Id,
            Request.PersonalInfoSection PersonalInfo,
            Request.Section<Request.ExperienceItemRequest> Experience,
            Request.Section<Request.ProjectItemRequest> Projects,
            Request.Section<Request.EducationItemRequest> Education,
            Request.SkillsetsSection Skillsets
        ) : ICommand<BaseResponseDto<UserCvDto>>;
    }
}
