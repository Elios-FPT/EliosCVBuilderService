using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Core.Handler.UserCv.Command
{
    public class CreateUserCvCommandHandler : ICommandHandler<CreateUserCvCommand, BaseResponseDto<UserCvDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> _templateRepository;

        public CreateUserCvCommandHandler(
            IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository,
            IGenericRepository<Elios.CVBuilder.Domain.Models.TemplateCv> templateRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        }

        public async Task<BaseResponseDto<UserCvDto>> Handle(CreateUserCvCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                return new BaseResponseDto<UserCvDto>
                {
                    Status = 400,
                    Message = "User ID cannot be empty.",
                    ResponseData = null
                };
            }

            if (request.TemplateId == Guid.Empty)
            {
                return new BaseResponseDto<UserCvDto>
                {
                    Status = 400,
                    Message = "Template ID cannot be empty.",
                    ResponseData = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return new BaseResponseDto<UserCvDto>
                {
                    Status = 400,
                    Message = "Title cannot be null or empty.",
                    ResponseData = null
                };
            }

            try
            {
                // Validate template exists and is not deleted
                var template = await _templateRepository.GetByIdAsync(request.TemplateId);
                if (template == null || template.IsDeleted)
                {
                    return new BaseResponseDto<UserCvDto>
                    {
                        Status = 404,
                        Message = "Template not found or has been deleted.",
                        ResponseData = null
                    };
                }

                using var transaction = await _userCvRepository.BeginTransactionAsync();
                try
                {
                    var userCv = new Elios.CVBuilder.Domain.Models.UserCv
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        TemplateId = request.TemplateId,
                        Title = request.Title,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null
                    };

                    await _userCvRepository.AddAsync(userCv);
                    await transaction.CommitAsync();

                    // Load template for response
                    userCv.Template = template;

                    return new BaseResponseDto<UserCvDto>
                    {
                        Status = 200,
                        Message = "User CV created successfully.",
                        ResponseData = userCv.ToDto()
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
                return new BaseResponseDto<UserCvDto>
                {
                    Status = 500,
                    Message = $"Failed to create user CV: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
