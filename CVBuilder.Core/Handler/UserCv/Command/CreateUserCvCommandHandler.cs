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
    public class CreateUserCvCommandHandler : ICommandHandler<CreateUserCvCommand, BaseResponseDto<CreateUserCvResponseDto>>
	{
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _resumeRepo;

		public CreateUserCvCommandHandler(
			IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> resumeRepo)
		{
			_resumeRepo = resumeRepo;
		}

        public async Task<BaseResponseDto<CreateUserCvResponseDto>> Handle(CreateUserCvCommand request, CancellationToken cancellationToken)
		{
            if (string.IsNullOrWhiteSpace(request.ResumeTitle))
			{
                return new BaseResponseDto<CreateUserCvResponseDto>
				{
					Status = 400,
                    Message = "Resume title is required.",
					ResponseData = null
				};
			}

            var resumeTitle = request.ResumeTitle.Trim();

            if (resumeTitle.Length == 0)
            {
                return new BaseResponseDto<CreateUserCvResponseDto>
                {
                    Status = 400,
                    Message = "Resume title cannot be empty.",
                    ResponseData = null
                };
            }

			try
			{
				using var uow = await _resumeRepo.BeginTransactionAsync();
				try
				{
					var resume = new Elios.CVBuilder.Domain.Models.UserCv
					{
						Id = Guid.NewGuid(),
                        OwnerId = request.OwnerId,
                        ResumeTitle = resumeTitle,
                        Data = GetDefaultCvData(),
						CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false,
                        DeletedAt = null
					};

					await _resumeRepo.AddAsync(resume);
					await uow.CommitAsync();

                    return new BaseResponseDto<CreateUserCvResponseDto>
					{
                        Status = 201,
                        Message = "User CV created successfully.",
                        ResponseData = new CreateUserCvResponseDto(resume.Id)
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
                return new BaseResponseDto<CreateUserCvResponseDto>
				{
					Status = 500,
					Message = $"Failed to create user CV: {ex.Message}",
					ResponseData = null
				};
			}
		}

		private static string GetDefaultCvData()
		{
			var defaultData = new
			{
				projects = new
				{
					id = "projects",
					items = new object[0],
					title = "Projects"
				},
				education = new
				{
					id = "education",
					items = new object[0],
					title = "Education"
				},
				skillsets = new
				{
					id = "skillsets",
					title = "Skillsets",
					tools = new
					{
						id = "tools",
						items = new object[0],
						title = "Tools / Platforms"
					},
					databases = new
					{
						id = "databases",
						items = new object[0],
						title = "Databases"
					},
					languages = new
					{
						id = "languages",
						items = new object[0],
						title = "Languages"
					},
					frameworks = new
					{
						id = "frameworks",
						items = new object[0],
						title = "Libraries / Frameworks"
					}
				},
				experience = new { }
			};

			return JsonSerializer.Serialize(defaultData, new JsonSerializerOptions
			{
				WriteIndented = false
			});
		}
	}
}
