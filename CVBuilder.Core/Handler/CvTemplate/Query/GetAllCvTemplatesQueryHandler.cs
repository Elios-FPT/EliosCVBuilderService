using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.CvTemplate.Query;

namespace CVBuilder.Core.Handler.CvTemplate.Query
{
    public class GetAllCvTemplatesQueryHandler : IQueryHandler<GetAllCvTemplatesQuery, BaseResponseDto<IEnumerable<CvTemplateDto>>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> _templateRepository;

        public GetAllCvTemplatesQueryHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> templateRepository)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        }

        public async Task<BaseResponseDto<IEnumerable<CvTemplateDto>>> Handle(GetAllCvTemplatesQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                return new BaseResponseDto<IEnumerable<CvTemplateDto>>
                {
                    Status = 400,
                    Message = "Page number and page size must be positive.",
                    ResponseData = null
                };
            }

            try
            {
                Expression<Func<Elios.CVBuilder.Domain.Models.TemplateCv, bool>>? filter = null;
                if (!request.IncludeDeleted)
                {
                    filter = t => !t.IsDeleted;
                }

                var templates = await _templateRepository.GetListAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(t => t.CreatedAt),
                    pageSize: request.PageSize,
                    pageNumber: request.PageNumber);

                var templateDtos = templates.Select(t => t.ToDto()).ToList();

                return new BaseResponseDto<IEnumerable<CvTemplateDto>>
                {
                    Status = 200,
                    Message = templates.Any() ? "CV templates retrieved successfully." : "No CV templates found.",
                    ResponseData = templateDtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<IEnumerable<CvTemplateDto>>
                {
                    Status = 500,
                    Message = $"Failed to retrieve CV templates: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
