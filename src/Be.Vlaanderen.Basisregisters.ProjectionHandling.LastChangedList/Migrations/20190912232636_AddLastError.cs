using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Migrations
{
    public partial class AddLastError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastError",
                schema: "Redis",
                table: "LastChangedList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LastChangedList_LastError",
                schema: "Redis",
                table: "LastChangedList",
                column: "LastError");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LastChangedList_LastError",
                schema: "Redis",
                table: "LastChangedList");

            migrationBuilder.DropColumn(
                name: "LastError",
                schema: "Redis",
                table: "LastChangedList");
        }
    }
}
