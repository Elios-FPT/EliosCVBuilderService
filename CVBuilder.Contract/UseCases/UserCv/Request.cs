using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Contract.UseCases.UserCv
{
    public static class Request
    {
        public record CreateUserCvRequest(
            [Required] string Body
        );

        public record UpdateUserCvRequest(
            [Required] string Title
        );

        public record GetUserCvsRequest(
            int PageNumber = 1,
            int PageSize = 20
        );

        // Section chung: id/title/items
        public record Section<TItem>(
            string Id,
            string Title,
            List<TItem> Items
        );

        // ---- Personal Info + Links ----
        public record PersonalInfoSection(
            Guid Id,
            string Title,
            string FirstName,
            string LastName,
            string Email,
            string Phone,
            string Address,
            string JobTitle,
            Section<LinkItem> Links
        );

        public record LinkItem(
            string Id,
            string Name,
            string Url
        );

        // ---- Items (thêm Id để khớp JSON của bạn) ----
        public record ExperienceItemRequest(
            string Id,
            string Employer,
            string JobTitle,
            string Start,
            string End,
            string Location,
            string Description
        );

        public record ProjectItemRequest(
            string Id,
            string ProjectName,
            string Role,
            string Description,
            string TechStack,
            string Achievements,
            string? GithubUrl,
            string? LiveUrl
        );

        public record EducationItemRequest(
            string Id,
            string Institution,
            string Location,
            string DegreeType,
            string FieldOfStudy,
            string Start,
            string Grad,
            string? Gpa
        );

        // ---- Skillsets gồm 4 subsection ----
        public record SkillsetsSection(
            string Id,
            string Title,
            Section<SkillItemRequest> Languages,
            Section<SkillItemRequest> Frameworks,
            Section<SkillItemRequest> Tools,
            Section<SkillItemRequest> Databases
        );

        public record SkillItemRequest(
            string Id,
            string Name
        );

        public record UpdateUserCvRequestV2(
            PersonalInfoSection PersonalInfo,
            Section<ExperienceItemRequest> Experience,
            Section<ProjectItemRequest> Projects,
            Section<EducationItemRequest> Education,
            SkillsetsSection Skillsets
        );

    }
}
