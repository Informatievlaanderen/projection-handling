using Microsoft.EntityFrameworkCore.Migrations;

namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Migrations
{
    public partial class AddLastChangedListIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LastChangedList_Position_LastPopulatedPosition_ErrorCount_LastError",
                schema: "Redis",
                table: "LastChangedList",
                columns: new[] { "Position", "LastPopulatedPosition", "ErrorCount", "LastError" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LastChangedList_Position_LastPopulatedPosition_ErrorCount_LastError",
                schema: "Redis",
                table: "LastChangedList");
        }
    }
}
