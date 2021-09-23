using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Morphic.Focus.Migrations
{
    public partial class BlockListAppWebsite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockListAppWebsites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IconLocation = table.Column<string>(type: "TEXT", nullable: true),
                    AppWebsiteName = table.Column<string>(type: "TEXT", nullable: false),
                    BlockListId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockListAppWebsites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockListAppWebsites_BlockLists_BlockListId",
                        column: x => x.BlockListId,
                        principalTable: "BlockLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockListAppWebsites_BlockListId",
                table: "BlockListAppWebsites",
                column: "BlockListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockListAppWebsites");
        }
    }
}
