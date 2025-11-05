using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCvs_TemplateCvs_TemplateId",
                table: "UserCvs");

            migrationBuilder.AlterColumn<Guid>(
                name: "TemplateId",
                table: "UserCvs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCvs_TemplateCvs_TemplateId",
                table: "UserCvs",
                column: "TemplateId",
                principalTable: "TemplateCvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCvs_TemplateCvs_TemplateId",
                table: "UserCvs");

            migrationBuilder.AlterColumn<Guid>(
                name: "TemplateId",
                table: "UserCvs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCvs_TemplateCvs_TemplateId",
                table: "UserCvs",
                column: "TemplateId",
                principalTable: "TemplateCvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
