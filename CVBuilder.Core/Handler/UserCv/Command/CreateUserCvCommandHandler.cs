using CVBuilder.Contract.Message;
using CVBuilder.Contract.Shared;
using CVBuilder.Contract.TransferObjects;
using CVBuilder.Core.Extensions;
using CVBuilder.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CVBuilder.Contract.UseCases.UserCv.Command;
using static CVBuilder.Contract.UseCases.UserCv.Request;

namespace CVBuilder.Core.Handler.UserCv.Command
{
	public class CreateUserCvCommandHandler : ICommandHandler<CreateUserCvCommand, BaseResponseDto<UserCvDto>>
	{
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> _resumeRepo;
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.PersonalInfo> _personalRepo;
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.Link> _linkRepo;
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.ExperienceItem> _expRepo;
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.ProjectItem> _projRepo;
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.EducationItem> _eduRepo;
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.SkillSet> _skillSetRepo;
		private readonly IGenericRepository<Elios.CVBuilder.Domain.Models.SkillItem> _skillItemRepo;

		public CreateUserCvCommandHandler(
			IGenericRepository<Elios.CVBuilder.Domain.Models.UserCv> resumeRepo,
			IGenericRepository<Elios.CVBuilder.Domain.Models.PersonalInfo> personalRepo,
			IGenericRepository<Elios.CVBuilder.Domain.Models.Link> linkRepo,
			IGenericRepository<Elios.CVBuilder.Domain.Models.ExperienceItem> expRepo,
			IGenericRepository<Elios.CVBuilder.Domain.Models.ProjectItem> projRepo,
			IGenericRepository<Elios.CVBuilder.Domain.Models.EducationItem> eduRepo,
			IGenericRepository<Elios.CVBuilder.Domain.Models.SkillSet> skillSetRepo,
			IGenericRepository<Elios.CVBuilder.Domain.Models.SkillItem> skillItemRepo)
		{
			_resumeRepo = resumeRepo;
			_personalRepo = personalRepo;
			_linkRepo = linkRepo;
			_expRepo = expRepo;
			_projRepo = projRepo;
			_eduRepo = eduRepo;
			_skillSetRepo = skillSetRepo;
			_skillItemRepo = skillItemRepo;
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
					var userId = request.PersonalInfo.Id;
					var resume = new Elios.CVBuilder.Domain.Models.UserCv
					{
						Id = Guid.NewGuid(),
						UserId = userId,
						ResumeTitle = request.PersonalInfo.Title,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = null
					};

					await _resumeRepo.AddAsync(resume);

					// Personal Info
					var p = request.PersonalInfo;
					var personal = new Elios.CVBuilder.Domain.Models.PersonalInfo
					{
						Id = Guid.NewGuid(),
						UserCvId = resume.Id,
						FirstName = p.FirstName,
						LastName = p.LastName,
						Email = p.Email,
						Phone = p.Phone,
						Address = p.Address,
						JobTitle = p.JobTitle
					};
					await _personalRepo.AddAsync(personal);

					if (p.Links?.Items != null)
					{
						foreach (var l in p.Links.Items)
						{
							await _linkRepo.AddAsync(new Elios.CVBuilder.Domain.Models.Link
							{
								Id = Guid.NewGuid(),
								PersonalInfoId = personal.Id,
								Name = l.Name,
								Url = l.Url
							});
						}
					}

					// Experience
					if (request.Experience?.Items != null)
					{
						int expIndex = 0;
						foreach (var e in request.Experience.Items)
						{
							await _expRepo.AddAsync(new Elios.CVBuilder.Domain.Models.ExperienceItem
							{
								Id = Guid.NewGuid(),
								UserCvId = resume.Id,
								Employer = e.Employer,
								JobTitle = e.JobTitle,
								Start = e.Start,
								End = e.End,
								Location = e.Location,
								Description = e.Description,
								OrderIndex = expIndex++
							});
						}
					}

					// Projects
					if (request.Projects?.Items != null)
					{
						int projIndex = 0;
						foreach (var pr in request.Projects.Items)
						{
							await _projRepo.AddAsync(new Elios.CVBuilder.Domain.Models.ProjectItem
							{
								Id = Guid.NewGuid(),
								UserCvId = resume.Id,
								ProjectName = pr.ProjectName,
								Role = pr.Role,
								Description = pr.Description,
								TechStack = pr.TechStack,
								Achievements = pr.Achievements,
								GithubUrl = pr.GithubUrl,
								LiveUrl = pr.LiveUrl,
								OrderIndex = projIndex++
							});
						}
					}

					// Education
					if (request.Education?.Items != null)
					{
						int eduIndex = 0;
						foreach (var ed in request.Education.Items)
						{
							await _eduRepo.AddAsync(new Elios.CVBuilder.Domain.Models.EducationItem
							{
								Id = Guid.NewGuid(),
								UserCvId = resume.Id,
								Institution = ed.Institution,
								Location = ed.Location,
								DegreeType = ed.DegreeType,
								FieldOfStudy = ed.FieldOfStudy,
								Start = ed.Start,
								Grad = ed.Grad,
								Gpa = ed.Gpa,
								OrderIndex = eduIndex++
							});
						}
					}

					// Skillsets
					if (request.Skillsets != null)
					{
						var skillSet = new Elios.CVBuilder.Domain.Models.SkillSet
						{
							Id = Guid.NewGuid(),
							UserCvId = resume.Id
						};
						await _skillSetRepo.AddAsync(skillSet);

						async Task addSkills(Section<SkillItemRequest>? section, Elios.CVBuilder.Domain.Models.SkillType type)
						{
							if (section?.Items == null) return;
							int idx = 0;
							foreach (var s in section.Items)
							{
								await _skillItemRepo.AddAsync(new Elios.CVBuilder.Domain.Models.SkillItem
								{
									Id = Guid.NewGuid(),
									SkillSetId = skillSet.Id,
									Type = type,
									Name = s.Name,
									OrderIndex = idx++
								});
							}
						}

						await addSkills(request.Skillsets.Languages, Elios.CVBuilder.Domain.Models.SkillType.Language);
						await addSkills(request.Skillsets.Frameworks, Elios.CVBuilder.Domain.Models.SkillType.Framework);
						await addSkills(request.Skillsets.Tools, Elios.CVBuilder.Domain.Models.SkillType.Tool);
						await addSkills(request.Skillsets.Databases, Elios.CVBuilder.Domain.Models.SkillType.Database);
					}

					await uow.CommitAsync();

					return new BaseResponseDto<UserCvDto>
					{
						Status = 200,
						Message = "User CV created successfully.",
						ResponseData = resume.ToDto()
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
