using System;
using System.Collections.Generic;

namespace CVBuilder.Contract.TransferObjects
{
    // Shared section container
    public record SectionDto<TItem>(
        Guid Id,
        string Title,
        List<TItem> Items
    );

    // Personal info and links
    public record PersonalInfoSectionDto(
        Guid Id,
        string Title,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string Address,
        string JobTitle,
        SectionDto<LinkItemDto> Links
    );

    public record LinkItemDto(
        Guid Id,
        string Name,
        string Url
    );

    // Items
    public record ExperienceItemDto(
        Guid Id,
        string Employer,
        string JobTitle,
        string Start,
        string End,
        string Location,
        string Description
    );

    public record ProjectItemDto(
        Guid Id,
        string ProjectName,
        string Role,
        string Description,
        string TechStack,
        string Achievements,
        string? GithubUrl,
        string? LiveUrl
    );

    public record EducationItemDto(
        Guid Id,
        string Institution,
        string Location,
        string DegreeType,
        string FieldOfStudy,
        string Start,
        string Grad,
        string? Gpa
    );

    // Skillsets
    public record SkillItemDto(
        Guid Id,
        string Name
    );

    public record SkillsetsSectionDto(
        Guid Id,
        string Title,
        SectionDto<SkillItemDto> Languages,
        SectionDto<SkillItemDto> Frameworks,
        SectionDto<SkillItemDto> Tools,
        SectionDto<SkillItemDto> Databases
    );

    // Top-level DTO mirrors CreateUserCvRequest / Sample_cv_body.json
    public class UserCvDto
    {
        public PersonalInfoSectionDto PersonalInfo { get; set; } = default!;
        public SectionDto<ExperienceItemDto> Experience { get; set; } = default!;
        public SectionDto<ProjectItemDto> Projects { get; set; } = default!;
        public SectionDto<EducationItemDto> Education { get; set; } = default!;
        public SkillsetsSectionDto Skillsets { get; set; } = default!;
    }
}
