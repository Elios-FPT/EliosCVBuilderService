using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Core.Handler.UserCv.Command
{
    public class UpdateUserCvCommandHandler : ICommandHandler<UpdateUserCvCommand, BaseResponseDto<UserCvDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public UpdateUserCvCommandHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<UserCvDto>> Handle(UpdateUserCvCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return new BaseResponseDto<UserCvDto>
                {
                    Status = 400,
                    Message = "User CV ID cannot be empty.",
                    ResponseData = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return new BaseResponseDto<UserCvDto>
                {
                    Status = 400,
                    Message = "ResumeTitle cannot be null or empty.",
                    ResponseData = null
                };
            }

            try
            {
                var existingUserCv = await _userCvRepository.GetOneAsync(
                    filter: uc => uc.Id == request.Id);

                if (existingUserCv == null)
                {
                    return new BaseResponseDto<UserCvDto>
                    {
                        Status = 404,
                        Message = "User CV not found.",
                        ResponseData = null
                    };
                }

                using var transaction = await _userCvRepository.BeginTransactionAsync();
                try
                {
                    existingUserCv.ResumeTitle = request.Title;
                    existingUserCv.UpdatedAt = DateTime.UtcNow;

                    await _userCvRepository.UpdateAsync(existingUserCv);
                    await transaction.CommitAsync();

                    return new BaseResponseDto<UserCvDto>
                    {
                        Status = 200,
                        Message = "User CV updated successfully.",
                        ResponseData = existingUserCv.ToDto()
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
                    Message = $"Failed to update user CV: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
