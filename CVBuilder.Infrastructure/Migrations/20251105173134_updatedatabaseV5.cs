using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabaseV5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationItem");

            migrationBuilder.DropTable(
                name: "ExperienceItem");

            migrationBuilder.DropTable(
                name: "Link");

            migrationBuilder.DropTable(
                name: "ProjectItem");

            migrationBuilder.DropTable(
                name: "SkillItem");

            migrationBuilder.DropTable(
                name: "PersonalInfo");

            migrationBuilder.DropTable(
                name: "SkillSet");

            // Some environments may already have removed this column; guard with IF EXISTS
            migrationBuilder.Sql("ALTER TABLE \"UserCvs\" DROP COLUMN IF EXISTS \"ResumeTitle\";");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserCvs",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCv_UserId",
                table: "UserCvs",
                newName: "IX_UserCv_OwnerId");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "UserCvs",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "UserCvs");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "UserCvs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCv_OwnerId",
                table: "UserCvs",
                newName: "IX_UserCv_UserId");

            migrationBuilder.AddColumn<string>(
                name: "ResumeTitle",
                table: "UserCvs",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EducationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserCvId = table.Column<Guid>(type: "uuid", nullable: false),
                    DegreeType = table.Column<string>(type: "text", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FieldOfStudy = table.Column<string>(type: "text", nullable: false),
                    Gpa = table.Column<string>(type: "text", nullable: true),
                    Grad = table.Column<string>(type: "text", nullable: false),
                    Institution = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationItem_UserCvs_UserCvId",
                        column: x => x.UserCvId,
                        principalTable: "UserCvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserCvId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Employer = table.Column<string>(type: "text", nullable: false),
                    End = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    JobTitle = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperienceItem_UserCvs_UserCvId",
                        column: x => x.UserCvId,
                        principalTable: "UserCvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserCvId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    JobTitle = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalInfo_UserCvs_UserCvId",
                        column: x => x.UserCvId,
                        principalTable: "UserCvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserCvId = table.Column<Guid>(type: "uuid", nullable: false),
                    Achievements = table.Column<string>(type: "text", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    GithubUrl = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LiveUrl = table.Column<string>(type: "text", nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    TechStack = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectItem_UserCvs_UserCvId",
                        column: x => x.UserCvId,
                        principalTable: "UserCvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserCvId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillSet_UserCvs_UserCvId",
                        column: x => x.UserCvId,
                        principalTable: "UserCvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Link",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonalInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Link", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Link_PersonalInfo_PersonalInfoId",
                        column: x => x.PersonalInfoId,
                        principalTable: "PersonalInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillItem_SkillSet_SkillSetId",
                        column: x => x.SkillSetId,
                        principalTable: "SkillSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationItem_UserCvId",
                table: "EducationItem",
                column: "UserCvId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceItem_UserCvId",
                table: "ExperienceItem",
                column: "UserCvId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_PersonalInfoId",
                table: "Link",
                column: "PersonalInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInfo_UserCvId",
                table: "PersonalInfo",
                column: "UserCvId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectItem_UserCvId",
                table: "ProjectItem",
                column: "UserCvId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillItem_SkillSetId",
                table: "SkillItem",
                column: "SkillSetId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillSet_UserCvId",
                table: "SkillSet",
                column: "UserCvId",
                unique: true);
        }
    }
}
