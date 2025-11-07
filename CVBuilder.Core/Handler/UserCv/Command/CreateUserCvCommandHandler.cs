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
            var data = new
            {
                personalInfo = new
                {
                    id = "personalInfo",
                    title = "Personal Info",
                    firstName = "Example",
                    lastName = "CV",
                    email = "examplecv@gmail.com",
                    phone = "+1 206 555 0100",
                    address = "123 Main Street, New York",
                    jobTitle = "Full-Stack Web Developer",
                    links = new
                    {
                        id = "links",
                        title = "Links",
                        items = new[]
                        {
                    new {
                        id = "a1b2c3d4-e5f6-7890-1234-567890abcdef",
                        name = "GitHub",
                        url = "https://github.com/yourprofile"
                    },
                    new {
                        id = "b2c3d4e5-f6a7-8901-2345-67890abcdef1",
                        name = "LinkedIn",
                        url = "https://linkedin.com/in/yourprofile"
                    }
                }
                    }
                },
                experience = new
                {
                    id = "experience",
                    title = "Experience",
                    items = new[]
                    {
                new {
                    id = "c3d4e5f6-a7b8-9012-3456-7890abcdef12",
                    employer = "Tech Solutions Inc.",
                    jobTitle = "Senior Software Engineer",
                    start = "01/2022",
                    end = "Present",
                    location = "San Francisco, CA",
                    description = "Led the development of a new client-facing dashboard using React and Node.js.\n- Mentored junior developers.\n- Improved API response times by 30%."
                }
            }
                },
                projects = new
                {
                    id = "projects",
                    title = "Projects",
                    items = new[]
                    {
                new {
                    id = "d4e5f6a7-b8c9-0123-4567-890abcdef123",
                    projectName = "Resume Builder",
                    role = "Lead Developer",
                    description = "A web application that allows users to create, edit, and export professional resumes.",
                    techStack = "React, Node.js, Express, MongoDB",
                    achievements = "- Implemented real-time preview functionality.\n- Designed a flexible component-based editor.\n- Integrated PDF export feature.",
                    githubUrl = "https://github.com/yourprofile/resume-builder",
                    liveUrl = "https://your-resume-builder-demo.com"
                }
            }
                },
                education = new
                {
                    id = "education",
                    title = "Education",
                    items = new[]
                    {
                new {
                    id = "e5f6a7b8-c9d0-1234-5678-90abcdef1234",
                    institution = "University of Technology",
                    location = "New York, NY",
                    degreeType = "Bachelor of Science",
                    fieldOfStudy = "Computer Science",
                    start = "08/2018",
                    grad = "05/2022",
                    gpa = "3.8"
                }
            }
                },
                skillsets = new
                {
                    id = "skillsets",
                    title = "Skillsets",
                    languages = new
                    {
                        id = "languages",
                        title = "Languages",
                        items = new[]
                        {
                    new { id = "f6a7b8c9-d0e1-2345-6789-0abcdef12345", name = "JavaScript" },
                    new { id = "a7b8c9d0-e1f2-3456-7890-bcdef1234567", name = "Python" }
                }
                    },
                    frameworks = new
                    {
                        id = "frameworks",
                        title = "Libraries / Frameworks",
                        items = new[]
                        {
                    new { id = "b8c9d0e1-f2a3-4567-8901-cdef12345678", name = "React" },
                    new { id = "c9d0e1f2-a3b4-5678-9012-def123456789", name = "Node.js" },
                    new { id = "d0e1f2a3-b4c5-6789-0123-ef1234567890", name = "Express.js" }
                }
                    },
                    tools = new
                    {
                        id = "tools",
                        title = "Tools / Platforms",
                        items = new[]
                        {
                    new { id = "e1f2a3b4-c5d6-7890-1234-f12345678901", name = "Git & GitHub" },
                    new { id = "f2a3b4c5-d6e7-8901-2345-123456789012", name = "Docker" }
                }
                    },
                    databases = new
                    {
                        id = "databases",
                        title = "Databases",
                        items = new[]
                        {
                    new { id = "a3b4c5d6-e7f8-9012-3456-234567890123", name = "MongoDB" },
                    new { id = "b4c5d6e7-f8a9-0123-4567-345678901234", name = "PostgreSQL" }
                }
                    }
                }
            };

            return JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

    }
}
