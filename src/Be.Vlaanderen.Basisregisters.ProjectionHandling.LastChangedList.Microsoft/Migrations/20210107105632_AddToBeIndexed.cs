using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Migrations
{
    public partial class AddToBeIndexed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ToBeIndexed",
                schema: "Redis",
                table: "LastChangedList",
                type: "bit",
                nullable: false,
                computedColumnSql: "CAST(CASE WHEN (([Position] > [LastPopulatedPosition]) AND ([ErrorCount] < 10)) THEN 1 ELSE 0 END AS bit) PERSISTED");

            migrationBuilder.CreateIndex(
                name: "IX_LastChangedList_ToBeIndexed",
                schema: "Redis",
                table: "LastChangedList",
                column: "ToBeIndexed")
                .Annotation("SqlServer:Include", new[] { "LastError" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LastChangedList_ToBeIndexed",
                schema: "Redis",
                table: "LastChangedList");

            migrationBuilder.DropColumn(
                name: "ToBeIndexed",
                schema: "Redis",
                table: "LastChangedList");
        }
    }
}
