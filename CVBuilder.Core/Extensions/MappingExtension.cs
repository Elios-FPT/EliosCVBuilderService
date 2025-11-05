using CVBuilder.Contract.TransferObjects;
using CVBuilder.Domain.Entities;
using System.Linq;

namespace CVBuilder.Core.Extensions
{
    public static class MappingExtension
    {
        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                Url = notification.Url,
                Metadata = notification.Metadata
            };
        }

        public static CvTemplateDto ToDto(this Elios.CVBuilder.Domain.Models.TemplateCv templateCv)
        {
            return new CvTemplateDto
            {
                Id = templateCv.Id,
                Name = templateCv.Name,
                Description = templateCv.Description,
                ThumbnailUrl = templateCv.ThumbnailUrl,
                CreatedAt = templateCv.CreatedAt,
                IsDeleted = templateCv.IsDeleted
            };
        }

        public static UserCvDto ToDto(this Elios.CVBuilder.Domain.Models.UserCv userCv)
        {
            // PersonalInfo + Links
            var links = (userCv.PersonalInfo?.Links ?? new System.Collections.Generic.List<Elios.CVBuilder.Domain.Models.Link>())
                .Select(l => new LinkItemDto(l.Id, l.Name, l.Url))
                .ToList();

            var personalInfo = new PersonalInfoSectionDto(
                userCv.UserId,
                userCv.ResumeTitle,
                userCv.PersonalInfo?.FirstName ?? string.Empty,
                userCv.PersonalInfo?.LastName ?? string.Empty,
                userCv.PersonalInfo?.Email ?? string.Empty,
                userCv.PersonalInfo?.Phone ?? string.Empty,
                userCv.PersonalInfo?.Address ?? string.Empty,
                userCv.PersonalInfo?.JobTitle ?? string.Empty,
                new SectionDto<LinkItemDto>(
                    Id: userCv.PersonalInfo?.Links != null ? (userCv.PersonalInfo.Links.FirstOrDefault()?.Id ?? System.Guid.NewGuid()) : System.Guid.NewGuid(),
                    Title: "Links",
                    Items: links)
            );

            // Experience
            var expItems = (userCv.ExperienceItems ?? new System.Collections.Generic.List<Elios.CVBuilder.Domain.Models.ExperienceItem>())
                .OrderBy(e => e.OrderIndex)
                .Select(e => new ExperienceItemDto(e.Id, e.Employer, e.JobTitle, e.Start, e.End, e.Location, e.Description))
                .ToList();
            var experience = new SectionDto<ExperienceItemDto>(System.Guid.NewGuid(), "Experience", expItems);

            // Projects
            var projItems = (userCv.ProjectItems ?? new System.Collections.Generic.List<Elios.CVBuilder.Domain.Models.ProjectItem>())
                .OrderBy(p => p.OrderIndex)
                .Select(p => new ProjectItemDto(p.Id, p.ProjectName, p.Role, p.Description, p.TechStack, p.Achievements, p.GithubUrl, p.LiveUrl))
                .ToList();
            var projects = new SectionDto<ProjectItemDto>(System.Guid.NewGuid(), "Projects", projItems);

            // Education
            var eduItems = (userCv.EducationItems ?? new System.Collections.Generic.List<Elios.CVBuilder.Domain.Models.EducationItem>())
                .OrderBy(e => e.OrderIndex)
                .Select(e => new EducationItemDto(e.Id, e.Institution, e.Location, e.DegreeType, e.FieldOfStudy, e.Start, e.Grad, e.Gpa))
                .ToList();
            var education = new SectionDto<EducationItemDto>(System.Guid.NewGuid(), "Education", eduItems);

            // Skillsets: build subsections from items by type
            var skillItems = (userCv.SkillSet?.SkillItems ?? new System.Collections.Generic.List<Elios.CVBuilder.Domain.Models.SkillItem>()).OrderBy(s => s.OrderIndex).ToList();
            System.Func<Elios.CVBuilder.Domain.Models.SkillType, SectionDto<SkillItemDto>> buildSkills = (type) =>
            {
                var list = skillItems.Where(s => s.Type == type).Select(s => new SkillItemDto(s.Id, s.Name)).ToList();
                var title = type switch
                {
                    Elios.CVBuilder.Domain.Models.SkillType.Language => "Languages",
                    Elios.CVBuilder.Domain.Models.SkillType.Framework => "Libraries / Frameworks",
                    Elios.CVBuilder.Domain.Models.SkillType.Tool => "Tools / Platforms",
                    Elios.CVBuilder.Domain.Models.SkillType.Database => "Databases",
                    _ => "Skills"
                };
                return new SectionDto<SkillItemDto>(System.Guid.NewGuid(), title, list);
            };

            var skillsets = new SkillsetsSectionDto(
                System.Guid.NewGuid(),
                "Skillsets",
                buildSkills(Elios.CVBuilder.Domain.Models.SkillType.Language),
                buildSkills(Elios.CVBuilder.Domain.Models.SkillType.Framework),
                buildSkills(Elios.CVBuilder.Domain.Models.SkillType.Tool),
                buildSkills(Elios.CVBuilder.Domain.Models.SkillType.Database)
            );

            return new UserCvDto
            {
                PersonalInfo = personalInfo,
                Experience = experience,
                Projects = projects,
                Education = education,
                Skillsets = skillsets
            };
        }
    }
}
