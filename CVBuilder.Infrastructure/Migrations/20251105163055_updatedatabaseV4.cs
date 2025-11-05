using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabaseV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "SkillSet");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "PersonalInfo");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserCvs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserCvs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SkillSet",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SkillSet",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SkillItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SkillItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProjectItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProjectItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PersonalInfo",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PersonalInfo",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Link",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Link",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExperienceItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExperienceItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EducationItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EducationItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserCv_UserId",
                table: "UserCvs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCv_UserId",
                table: "UserCvs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserCvs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserCvs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SkillSet");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SkillSet");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SkillItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SkillItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProjectItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProjectItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PersonalInfo");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PersonalInfo");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Link");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Link");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExperienceItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExperienceItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EducationItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EducationItem");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "SkillSet",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "PersonalInfo",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
