using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Interfaces;
using Elios.CVBuilder.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Core.Handler.UserCv.Command
{
    public class DeleteUserCvCommandHandler : ICommandHandler<DeleteUserCvCommand, BaseResponseDto<DeleteUserCvResponseDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public DeleteUserCvCommandHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<DeleteUserCvResponseDto>> Handle(DeleteUserCvCommand request, CancellationToken cancellationToken)
        {
            if (request.IdHeader == Guid.Empty)
            {
                return new BaseResponseDto<DeleteUserCvResponseDto>
                {
                    Status = 400,
                    Message = "User ID cannot be empty.",
                    ResponseData = new DeleteUserCvResponseDto(false, "User ID cannot be empty.")
                };
            }

            if (request.Id == Guid.Empty)
            {
                return new BaseResponseDto<DeleteUserCvResponseDto>
                {
                    Status = 400,
                    Message = "User CV ID cannot be empty.",
                    ResponseData = new DeleteUserCvResponseDto(false, "User CV ID cannot be empty.")
                };
            }

            try
            {
                var existingUserCv = await _userCvRepository.GetByIdAsync(request.Id);
                if (existingUserCv == null)
                {
                    return new BaseResponseDto<DeleteUserCvResponseDto>
                    {
                        Status = 404,
                        Message = "User CV not found.",
                        ResponseData = new DeleteUserCvResponseDto(false, "User CV not found.")
                    };
                }

                if (existingUserCv.OwnerId != request.IdHeader)
                {
                    return new BaseResponseDto<DeleteUserCvResponseDto>
                    {
                        Status = 403,
                        Message = "You can only delete your own resume.",
                        ResponseData = new DeleteUserCvResponseDto(false, "You can only delete your own resume.")
                    };
                }

                // Check if already deleted
                if (existingUserCv.IsDeleted)
                {
                    return new BaseResponseDto<DeleteUserCvResponseDto>
                    {
                        Status = 400,
                        Message = "User CV is already deleted.",
                        ResponseData = new DeleteUserCvResponseDto(false, "User CV is already deleted.")
                    };
                }

                using var transaction = await _userCvRepository.BeginTransactionAsync();
                try
                {
                    // Soft delete: set IsDeleted = true and DeletedAt = current time
                    existingUserCv.IsDeleted = true;
                    existingUserCv.DeletedAt = DateTime.UtcNow;
                    
                    await _userCvRepository.DeleteAsync(existingUserCv);
                    await transaction.CommitAsync();

                    return new BaseResponseDto<DeleteUserCvResponseDto>
                    {
                        Status = 200,
                        Message = "User CV deleted successfully.",
                        ResponseData = new DeleteUserCvResponseDto(true, "Resume deleted successfully.")
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
                return new BaseResponseDto<DeleteUserCvResponseDto>
                {
                    Status = 500,
                    Message = $"Failed to delete user CV: {ex.Message}",
                    ResponseData = new DeleteUserCvResponseDto(false, "Failed to delete user CV.")
                };
            }
        }
    }
}
