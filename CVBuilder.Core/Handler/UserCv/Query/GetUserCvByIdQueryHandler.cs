using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Core.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Core.Handler.UserCv.Query
{
    public class GetUserCvByIdQueryHandler : IQueryHandler<GetUserCvByIdQuery, BaseResponseDto<JsonElement>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public GetUserCvByIdQueryHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<JsonElement>> Handle(GetUserCvByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return new BaseResponseDto<JsonElement>
                {
                    Status = 400,
                    Message = "User CV ID cannot be empty.",
                    ResponseData = default
                };
            }

            try
            {
                var userCv = await _userCvRepository.GetOneAsync(
                    filter: uc => uc.Id == request.Id);

                if (userCv == null)
                {
                    return new BaseResponseDto<JsonElement>
                    {
                        Status = 404,
                        Message = "User CV not found.",
                        ResponseData = default
                    };
                }

                if(request.OwnerId != userCv.OwnerId)
                {
                    return new BaseResponseDto<JsonElement>
                    {
                        Status = 404,
                        Message = "You can only watch own resume.",
                    };
                }

                if (string.IsNullOrWhiteSpace(userCv.Body))
                {
                    return new BaseResponseDto<JsonElement>
                    {
                        Status = 404,
                        Message = "User CV body is empty.",
                        ResponseData = default
                    };
                }

                // Parse JSON string to JsonElement
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(userCv.Body);

                return new BaseResponseDto<JsonElement>
                {
                    Status = 200,
                    Message = "User CV retrieved successfully.",
                    ResponseData = jsonElement
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<JsonElement>
                {
                    Status = 500,
                    Message = $"Failed to retrieve user CV: {ex.Message}",
                    ResponseData = default
                };
            }
        }
    }
}
