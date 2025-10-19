using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.CvTemplate.Query;

namespace CVBuilder.Core.Handler.CvTemplate.Query
{
    public class GetCvTemplateByIdQueryHandler : IQueryHandler<GetCvTemplateByIdQuery, BaseResponseDto<CvTemplateDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> _templateRepository;

        public GetCvTemplateByIdQueryHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> templateRepository)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        }

        public async Task<BaseResponseDto<CvTemplateDto>> Handle(GetCvTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return new BaseResponseDto<CvTemplateDto>
                {
                    Status = 400,
                    Message = "Template ID cannot be empty.",
                    ResponseData = null
                };
            }

            try
            {
                var template = await _templateRepository.GetOneAsync(
                    filter: t => t.Id == request.Id && !t.IsDeleted);

                if (template == null)
                {
                    return new BaseResponseDto<CvTemplateDto>
                    {
                        Status = 404,
                        Message = "CV template not found.",
                        ResponseData = null
                    };
                }

                return new BaseResponseDto<CvTemplateDto>
                {
                    Status = 200,
                    Message = "CV template retrieved successfully.",
                    ResponseData = template.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<CvTemplateDto>
                {
                    Status = 500,
                    Message = $"Failed to retrieve CV template: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
