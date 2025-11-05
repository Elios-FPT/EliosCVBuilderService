using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropTitleColumnFromUserCvs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop Title column if it exists (created in initDB migration)
            migrationBuilder.Sql("ALTER TABLE \"UserCvs\" DROP COLUMN IF EXISTS \"Title\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore Title column if migration is rolled back
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "UserCvs",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
