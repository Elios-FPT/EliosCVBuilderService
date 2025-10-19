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
    public static class Query
    {
        public record GetCvTemplateByIdQuery(
            Guid Id
        ) : IQuery<BaseResponseDto<CvTemplateDto>>;

        public record GetAllCvTemplatesQuery(
            int PageNumber = 1,
            int PageSize = 20,
            bool IncludeDeleted = false
        ) : IQuery<BaseResponseDto<IEnumerable<CvTemplateDto>>>;
    }
}
