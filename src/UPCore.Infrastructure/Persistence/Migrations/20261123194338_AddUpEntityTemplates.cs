using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUpEntityTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UpEntityTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fields = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpEntityTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UpEntityTemplates_Name",
                table: "UpEntityTemplates",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UpEntityTemplates");
        }
    }
}
