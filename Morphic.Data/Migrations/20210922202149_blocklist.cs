using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Morphic.Focus.Migrations
{
    public partial class blocklist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnblockItemId",
                table: "UnblockItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralSettingId",
                table: "GeneralSettings",
                newName: "Id");

            migrationBuilder.CreateTable(
                name: "BlockLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockLists", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockLists");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UnblockItems",
                newName: "UnblockItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GeneralSettings",
                newName: "GeneralSettingId");
        }
    }
}
