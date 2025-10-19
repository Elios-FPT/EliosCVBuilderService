using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Core.Handler.UserCv.Query
{
    public class GetUserCvsQueryHandler : IQueryHandler<GetUserCvsQuery, BaseResponseDto<IEnumerable<UserCvDto>>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public GetUserCvsQueryHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<IEnumerable<UserCvDto>>> Handle(GetUserCvsQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                return new BaseResponseDto<IEnumerable<UserCvDto>>
                {
                    Status = 400,
                    Message = "User ID cannot be empty.",
                    ResponseData = null
                };
            }

            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                return new BaseResponseDto<IEnumerable<UserCvDto>>
                {
                    Status = 400,
                    Message = "Page number and page size must be positive.",
                    ResponseData = null
                };
            }

            try
            {
                var userCvs = await _userCvRepository.GetListAsync(
                    filter: uc => uc.UserId == request.UserId,
                    orderBy: q => q.OrderByDescending(uc => uc.CreatedAt),
                    include: q => q.Include(uc => uc.Template),
                    pageSize: request.PageSize,
                    pageNumber: request.PageNumber);

                var userCvDtos = userCvs.Select(uc => uc.ToDto()).ToList();

                return new BaseResponseDto<IEnumerable<UserCvDto>>
                {
                    Status = 200,
                    Message = userCvs.Any() ? "User CVs retrieved successfully." : "No user CVs found.",
                    ResponseData = userCvDtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<IEnumerable<UserCvDto>>
                {
                    Status = 500,
                    Message = $"Failed to retrieve user CVs: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
