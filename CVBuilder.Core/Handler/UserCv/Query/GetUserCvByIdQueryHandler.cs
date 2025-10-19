using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Core.Handler.UserCv.Query
{
    public class GetUserCvByIdQueryHandler : IQueryHandler<GetUserCvByIdQuery, BaseResponseDto<UserCvDto>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public GetUserCvByIdQueryHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<UserCvDto>> Handle(GetUserCvByIdQuery request, CancellationToken cancellationToken)
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

            try
            {
                var userCv = await _userCvRepository.GetOneAsync(
                    filter: uc => uc.Id == request.Id,
                    include: q => q.Include(uc => uc.Template));

                if (userCv == null)
                {
                    return new BaseResponseDto<UserCvDto>
                    {
                        Status = 404,
                        Message = "User CV not found.",
                        ResponseData = null
                    };
                }

                return new BaseResponseDto<UserCvDto>
                {
                    Status = 200,
                    Message = "User CV retrieved successfully.",
                    ResponseData = userCv.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<UserCvDto>
                {
                    Status = 500,
                    Message = $"Failed to retrieve user CV: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
