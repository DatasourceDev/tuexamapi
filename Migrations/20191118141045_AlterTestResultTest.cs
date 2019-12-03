using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTestResultTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestID",
                table: "TestResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestID",
                table: "TestResults",
                column: "TestID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestID",
                table: "TestResults",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestID",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_TestID",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "TestID",
                table: "TestResults");
        }
    }
}
