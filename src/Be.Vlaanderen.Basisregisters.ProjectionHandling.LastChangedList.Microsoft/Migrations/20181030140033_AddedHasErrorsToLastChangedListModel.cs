using Microsoft.EntityFrameworkCore.Migrations;

namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Migrations
{
    public partial class AddedHasErrorsToLastChangedListModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasErrors",
                schema: "Redis",
                table: "LastChangedList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_LastChangedList_HasErrors",
                schema: "Redis",
                table: "LastChangedList",
                column: "HasErrors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LastChangedList_HasErrors",
                schema: "Redis",
                table: "LastChangedList");

            migrationBuilder.DropColumn(
                name: "HasErrors",
                schema: "Redis",
                table: "LastChangedList");
        }
    }
}
