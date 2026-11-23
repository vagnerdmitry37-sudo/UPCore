using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameUpEntityTemplateNameToType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UpEntityTemplates",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_UpEntityTemplates_Name",
                table: "UpEntityTemplates",
                newName: "IX_UpEntityTemplates_Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "UpEntityTemplates",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_UpEntityTemplates_Type",
                table: "UpEntityTemplates",
                newName: "IX_UpEntityTemplates_Name");
        }
    }
}
