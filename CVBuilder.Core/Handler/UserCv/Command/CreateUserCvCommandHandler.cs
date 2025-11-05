using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Command;

namespace CVBuilder.Core.Handler.UserCv.Command
{
	public class CreateUserCvCommandHandler : ICommandHandler<CreateUserCvCommand, BaseResponseDto<UserCvDto>>
	{
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _resumeRepo;

		public CreateUserCvCommandHandler(
			IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> resumeRepo)
		{
			_resumeRepo = resumeRepo;
		}

		public async Task<BaseResponseDto<UserCvDto>> Handle(CreateUserCvCommand request, CancellationToken cancellationToken)
		{
			if (request.PersonalInfo == null)
			{
				return new BaseResponseDto<UserCvDto>
				{
					Status = 400,
					Message = "PersonalInfo is required.",
					ResponseData = null
				};
			}

			try
			{
				using var uow = await _resumeRepo.BeginTransactionAsync();
				try
				{
					var ownerId = request.PersonalInfo.Id;
					
					// Serialize entire request to JSON
					var jsonBody = JsonSerializer.Serialize(request, new JsonSerializerOptions 
					{ 
						WriteIndented = false,
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					});

					var resume = new Elios.CVBuilder.Domain.Models.UserCv
					{
						Id = Guid.NewGuid(),
						OwnerId = ownerId,
						Body = jsonBody,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = null,
						IsDeleted = false
					};

					await _resumeRepo.AddAsync(resume);
					await uow.CommitAsync();

					// Deserialize Body back to DTO for response
					var responseDto = JsonSerializer.Deserialize<UserCvDto>(jsonBody, new JsonSerializerOptions 
					{ 
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
					});

					return new BaseResponseDto<UserCvDto>
					{
						Status = 200,
						Message = "User CV created successfully.",
						ResponseData = responseDto
					};
				}
				catch
				{
					await uow.RollbackAsync();
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
