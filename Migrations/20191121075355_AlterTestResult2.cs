using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTestResult2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestID",
                table: "TestResultStudents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudents_TestID",
                table: "TestResultStudents",
                column: "TestID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultStudents_Tests_TestID",
                table: "TestResultStudents",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResultStudents_Tests_TestID",
                table: "TestResultStudents");

            migrationBuilder.DropIndex(
                name: "IX_TestResultStudents_TestID",
                table: "TestResultStudents");

            migrationBuilder.DropColumn(
                name: "TestID",
                table: "TestResultStudents");
        }
    }
}
