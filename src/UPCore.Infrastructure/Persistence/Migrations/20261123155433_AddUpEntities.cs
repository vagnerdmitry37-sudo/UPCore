using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUpEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UpEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UpEntities_Type",
                table: "UpEntities",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UpEntities");
        }
    }
}
