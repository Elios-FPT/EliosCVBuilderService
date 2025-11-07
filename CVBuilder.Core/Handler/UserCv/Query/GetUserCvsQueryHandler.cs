using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Interfaces;
using Elios.CVBuilder.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Core.Handler.UserCv.Query
{
    public class GetUserCvsQueryHandler : IQueryHandler<GetUserCvsQuery, BaseResponseDto<IEnumerable<UserCvSummaryDto>>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public GetUserCvsQueryHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<IEnumerable<UserCvSummaryDto>>> Handle(GetUserCvsQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                return new BaseResponseDto<IEnumerable<UserCvSummaryDto>>
                {
                    Status = 400,
                    Message = "User ID cannot be empty.",
                    ResponseData = null
                };
            }

            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                return new BaseResponseDto<IEnumerable<UserCvSummaryDto>>
                {
                    Status = 400,
                    Message = "Page number and page size must be positive.",
                    ResponseData = null
                };
            }

            try
            {
                var userCvs = await _userCvRepository.GetListAsync(
                    filter: uc => uc.OwnerId == request.UserId,
                    orderBy: q => q.OrderByDescending(uc => uc.CreatedAt),
                    pageSize: request.PageSize,
                    pageNumber: request.PageNumber);

                var summaries = (userCvs ?? Enumerable.Empty<Elios.CVBuilder.Domain.Models.UserCv>())
                    .Select(uc => new UserCvSummaryDto(
                        uc.Id,
                        uc.ResumeTitle,
                        uc.UpdatedAt ?? uc.CreatedAt))
                    .ToList();

                return new BaseResponseDto<IEnumerable<UserCvSummaryDto>>
                {
                    Status = 200,
                    Message = summaries.Any() ? "User CVs retrieved successfully." : "No user CVs found.",
                    ResponseData = summaries
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<IEnumerable<UserCvSummaryDto>>
                {
                    Status = 500,
                    Message = $"Failed to retrieve user CVs: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
