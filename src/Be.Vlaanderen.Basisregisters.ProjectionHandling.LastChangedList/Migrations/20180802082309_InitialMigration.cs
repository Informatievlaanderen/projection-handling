using Microsoft.EntityFrameworkCore.Migrations;

namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: LastChangedListContext.Schema);

            migrationBuilder.CreateTable(
                name: LastChangedListContext.TableName,
                schema: LastChangedListContext.Schema,
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CacheKey = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    AcceptType = table.Column<string>(nullable: true),
                    Position = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastChangedList", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionStates",
                schema: LastChangedListContext.Schema,
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Position = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionStates", x => x.Name)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LastChangedList_CacheKey",
                schema: LastChangedListContext.Schema,
                table: LastChangedListContext.TableName,
                column: "CacheKey");

            migrationBuilder.CreateIndex(
                name: "IX_LastChangedList_Position",
                schema: LastChangedListContext.Schema,
                table: LastChangedListContext.TableName,
                column: "Position");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: LastChangedListContext.TableName,
                schema: LastChangedListContext.Schema);

            migrationBuilder.DropTable(
                name: "ProjectionStates",
                schema: LastChangedListContext.Schema);
        }
    }
}
