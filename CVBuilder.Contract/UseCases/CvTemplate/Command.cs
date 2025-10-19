using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Contract.UseCases.CvTemplate
{
    public static class Command
    {
        public record CreateCvTemplateCommand(
            string Name,
            string? Description,
            string? ThumbnailUrl
        ) : ICommand<BaseResponseDto<CvTemplateDto>>;

        public record UpdateCvTemplateCommand(
            Guid Id,
            string Name,
            string? Description,
            string? ThumbnailUrl
        ) : ICommand<BaseResponseDto<CvTemplateDto>>;

        public record DeleteCvTemplateCommand(
            Guid Id
        ) : ICommand<BaseResponseDto<bool>>;
    }
}
