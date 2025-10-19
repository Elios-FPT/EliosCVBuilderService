using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.CvTemplate.Command;

namespace CVBuilder.Core.Handler.CvTemplate.Command
{
    public class UpdateCvTemplateCommandHandler : ICommandHandler<UpdateCvTemplateCommand, BaseResponseDto<CvTemplateDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> _templateRepository;

        public UpdateCvTemplateCommandHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> templateRepository)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        }

        public async Task<BaseResponseDto<CvTemplateDto>> Handle(UpdateCvTemplateCommand request, CancellationToken cancellationToken)
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

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new BaseResponseDto<CvTemplateDto>
                {
                    Status = 400,
                    Message = "Template name cannot be null or empty.",
                    ResponseData = null
                };
            }

            try
            {
                var existingTemplate = await _templateRepository.GetByIdAsync(request.Id);
                if (existingTemplate == null)
                {
                    return new BaseResponseDto<CvTemplateDto>
                    {
                        Status = 404,
                        Message = "CV template not found.",
                        ResponseData = null
                    };
                }

                using var transaction = await _templateRepository.BeginTransactionAsync();
                try
                {
                    existingTemplate.Name = request.Name;
                    existingTemplate.Description = request.Description;
                    existingTemplate.ThumbnailUrl = request.ThumbnailUrl;

                    await _templateRepository.UpdateAsync(existingTemplate);
                    await transaction.CommitAsync();

                    return new BaseResponseDto<CvTemplateDto>
                    {
                        Status = 200,
                        Message = "CV template updated successfully.",
                        ResponseData = existingTemplate.ToDto()
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<CvTemplateDto>
                {
                    Status = 500,
                    Message = $"Failed to update CV template: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
