using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Core.Handler.UserCv.Command
{
    public class DeleteUserCvCommandHandler : ICommandHandler<DeleteUserCvCommand, BaseResponseDto<bool>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public DeleteUserCvCommandHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<bool>> Handle(DeleteUserCvCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return new BaseResponseDto<bool>
                {
                    Status = 400,
                    Message = "User CV ID cannot be empty.",
                    ResponseData = false
                };
            }

            try
            {
                var existingUserCv = await _userCvRepository.GetByIdAsync(request.Id);
                if (existingUserCv == null)
                {
                    return new BaseResponseDto<bool>
                    {
                        Status = 404,
                        Message = "User CV not found.",
                        ResponseData = false
                    };
                }

                using var transaction = await _userCvRepository.BeginTransactionAsync();
                try
                {
                    await _userCvRepository.DeleteAsync(existingUserCv);
                    await transaction.CommitAsync();

                    return new BaseResponseDto<bool>
                    {
                        Status = 200,
                        Message = "User CV deleted successfully.",
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
                    Message = $"Failed to delete user CV: {ex.Message}",
                    ResponseData = false
                };
            }
        }
    }
}
