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
    public class CreateCvTemplateCommandHandler : ICommandHandler<CreateCvTemplateCommand, BaseResponseDto<CvTemplateDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> _templateRepository;

        public CreateCvTemplateCommandHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> templateRepository)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        }

        public async Task<BaseResponseDto<CvTemplateDto>> Handle(CreateCvTemplateCommand request, CancellationToken cancellationToken)
        {
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
                using var transaction = await _templateRepository.BeginTransactionAsync();
                try
                {
                    var template = new Elios.CVBuilder.Domain.Models.TemplateCv
                    {
                        Id = Guid.NewGuid(),
                        Name = request.Name,
                        Description = request.Description,
                        ThumbnailUrl = request.ThumbnailUrl,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await _templateRepository.AddAsync(template);
                    await transaction.CommitAsync();

                    return new BaseResponseDto<CvTemplateDto>
                    {
                        Status = 200,
                        Message = "CV template created successfully.",
                        ResponseData = template.ToDto()
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
                    Message = $"Failed to create CV template: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
