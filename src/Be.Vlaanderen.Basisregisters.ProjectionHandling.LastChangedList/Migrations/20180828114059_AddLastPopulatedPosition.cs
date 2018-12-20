using Microsoft.EntityFrameworkCore.Migrations;

namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Migrations
{
    public partial class AddLastPopulatedPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastPopulatedPosition",
                schema: LastChangedListContext.Schema,
                table: LastChangedListContext.TableName,
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPopulatedPosition",
                schema: LastChangedListContext.Schema,
                table: LastChangedListContext.TableName);
        }
    }
}
