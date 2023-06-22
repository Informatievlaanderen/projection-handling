using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Migrations
{
    public partial class AddToBeIndexed_LastErrorIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LastChangedList_ToBeIndexed_LastError",
                schema: "Redis",
                table: "LastChangedList",
                columns: new[] { "ToBeIndexed", "LastError" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LastChangedList_ToBeIndexed_LastError",
                schema: "Redis",
                table: "LastChangedList");
        }
    }
}
