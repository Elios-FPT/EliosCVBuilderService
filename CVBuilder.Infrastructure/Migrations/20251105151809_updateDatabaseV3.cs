using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDatabaseV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCvs_TemplateCvs_TemplateId",
                table: "UserCvs");

            migrationBuilder.DropIndex(
                name: "IX_UserCvs_TemplateId",
                table: "UserCvs");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "UserCvs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "UserCvs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCvs_TemplateId",
                table: "UserCvs",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCvs_TemplateCvs_TemplateId",
                table: "UserCvs",
                column: "TemplateId",
                principalTable: "TemplateCvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
