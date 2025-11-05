using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Core.Interfaces;
using Elios.CVBuilder.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Query;

namespace CVBuilder.Core.Handler.UserCv.Query
{
    public class GetUserCvsQueryHandler : IQueryHandler<GetUserCvsQuery, BaseResponseDto<IEnumerable<JsonElement>>>
    {
        private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _userCvRepository;

        public GetUserCvsQueryHandler(IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> userCvRepository)
        {
            _userCvRepository = userCvRepository ?? throw new ArgumentNullException(nameof(userCvRepository));
        }

        public async Task<BaseResponseDto<IEnumerable<JsonElement>>> Handle(GetUserCvsQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                return new BaseResponseDto<IEnumerable<JsonElement>>
                {
                    Status = 400,
                    Message = "User ID cannot be empty.",
                    ResponseData = null
                };
            }

            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                return new BaseResponseDto<IEnumerable<JsonElement>>
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

                if (request.UserId != userCvs.First().OwnerId)
                {
                    return new BaseResponseDto<IEnumerable<JsonElement>>
                    {
                        Status = 400,
                        Message = "You can only watch own resume.",
                        ResponseData = null
                    };
                }

                // Parse each JSON string to JsonElement
                var jsonElements = new List<JsonElement>();
                
                foreach (var userCv in userCvs.Where(uc => !string.IsNullOrWhiteSpace(uc.Body)))
                {
                    try
                    {
                        var bodyElement = JsonSerializer.Deserialize<JsonElement>(userCv.Body);
                        
                        // Check if Body contains a nested "body" property (legacy format)
                        if (bodyElement.ValueKind == JsonValueKind.Object && bodyElement.TryGetProperty("body", out var nestedBody))
                        {
                            // If nestedBody is a string, parse it; otherwise use it directly
                            if (nestedBody.ValueKind == JsonValueKind.String)
                            {
                                var parsedBody = JsonSerializer.Deserialize<JsonElement>(nestedBody.GetString()!);
                                jsonElements.Add(parsedBody);
                            }
                            else
                            {
                                jsonElements.Add(nestedBody);
                            }
                        }
                        else
                        {
                            // Body is already the CV content JSON
                            jsonElements.Add(bodyElement);
                        }
                    }
                    catch (JsonException)
                    {
                        // Skip invalid JSON entries
                        continue;
                    }
                }

                return new BaseResponseDto<IEnumerable<JsonElement>>
                {
                    Status = 200,
                    Message = userCvs.Any() ? "User CVs retrieved successfully." : "No user CVs found.",
                    ResponseData = jsonElements
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<IEnumerable<JsonElement>>
                {
                    Status = 500,
                    Message = $"Failed to retrieve user CVs: {ex.Message}",
                    ResponseData = null
                };
            }
        }
    }
}
