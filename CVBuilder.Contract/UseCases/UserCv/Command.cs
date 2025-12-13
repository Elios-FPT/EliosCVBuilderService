using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Contract.UseCases.UserCv
{
    public static class Command
    {
        public record UpdateUserCvCommand(
            [Required]
            Guid Id,
            [Required]
            Guid IdHeader,
            [Required]
            string Body
        ) : ICommand<BaseResponseDto<UpdateUserCvResponseDto>>;

        public record DeleteUserCvCommand(
            [Required]
            Guid IdHeader,
            [Required]
            Guid Id
        ) : ICommand<BaseResponseDto<DeleteUserCvResponseDto>>;

        public record CreateUserCvCommand(
            [Required]
            Guid OwnerId,
            [Required]
            [MaxLength(100)]
            string ResumeTitle
        ) : ICommand<BaseResponseDto<CreateUserCvResponseDto>>;

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
