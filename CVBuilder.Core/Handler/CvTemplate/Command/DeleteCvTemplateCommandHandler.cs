using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.CvTemplate.Command;

namespace CVBuilder.Core.Handler.CvTemplate.Command
{
    public class DeleteCvTemplateCommandHandler : ICommandHandler<DeleteCvTemplateCommand, BaseResponseDto<bool>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> _templateRepository;

        public DeleteCvTemplateCommandHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> templateRepository)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        }

        public async Task<BaseResponseDto<bool>> Handle(DeleteCvTemplateCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return new BaseResponseDto<bool>
                {
                    Status = 400,
                    Message = "Template ID cannot be empty.",
                    ResponseData = false
                };
            }

            try
            {
                var existingTemplate = await _templateRepository.GetByIdAsync(request.Id);
                if (existingTemplate == null)
                {
                    return new BaseResponseDto<bool>
                    {
                        Status = 404,
                        Message = "CV template not found.",
                        ResponseData = false
                    };
                }

                using var transaction = await _templateRepository.BeginTransactionAsync();
                try
                {
                    // Soft delete
                    existingTemplate.IsDeleted = true;
                    await _templateRepository.UpdateAsync(existingTemplate);
                    await transaction.CommitAsync();

                    return new BaseResponseDto<bool>
                    {
                        Status = 200,
                        Message = "CV template deleted successfully.",
                        ResponseData = true
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
                return new BaseResponseDto<bool>
                {
                    Status = 500,
                    Message = $"Failed to delete CV template: {ex.Message}",
                    ResponseData = false
                };
            }
        }
    }
}
