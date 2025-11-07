using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Core.Handler.UserCv.Command
{
    public class UpdateUserCvCommandHandler : ICommandHandler<UpdateUserCvCommand, BaseResponseDto<UpdateUserCvResponseDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public UpdateUserCvCommandHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<UpdateUserCvResponseDto>> Handle(UpdateUserCvCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return new BaseResponseDto<UpdateUserCvResponseDto>
                {
                    Status = 400,
                    Message = "User CV ID cannot be empty.",
                    ResponseData = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return new BaseResponseDto<UpdateUserCvResponseDto>
                {
                    Status = 400,
                    Message = "Data is required.",
                    ResponseData = null
                };
            }

            try
            {
                var existingUserCv = await _userCvRepository.GetOneAsync(
                    filter: uc => uc.Id == request.Id);

                if (existingUserCv == null)
                {
                    return new BaseResponseDto<UpdateUserCvResponseDto>
                    {
                        Status = 404,
                        Message = "User CV not found.",
                        ResponseData = null
                    };
                }

                using var transaction = await _userCvRepository.BeginTransactionAsync();
                try
                {
                    // Update Data with new JSON string
                    existingUserCv.Data = request.Body;
                    existingUserCv.UpdatedAt = DateTime.UtcNow;

                    await _userCvRepository.UpdateAsync(existingUserCv);
                    await transaction.CommitAsync();

                    return new BaseResponseDto<UpdateUserCvResponseDto>
                    {
                        Status = 200,
                        Message = "User CV updated successfully.",
                        ResponseData = new UpdateUserCvResponseDto(true, "Resume updated successfully.")
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
                return new BaseResponseDto<UpdateUserCvResponseDto>
                {
                    Status = 500,
                    Message = $"Failed to update user CV: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
