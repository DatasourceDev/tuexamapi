using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTestResultStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestResultID",
                table: "TestResultStudents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudents_TestResultID",
                table: "TestResultStudents",
                column: "TestResultID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultStudents_TestResults_TestResultID",
                table: "TestResultStudents",
                column: "TestResultID",
                principalTable: "TestResults",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResultStudents_TestResults_TestResultID",
                table: "TestResultStudents");

            migrationBuilder.DropIndex(
                name: "IX_TestResultStudents_TestResultID",
                table: "TestResultStudents");

            migrationBuilder.DropColumn(
                name: "TestResultID",
                table: "TestResultStudents");
        }
    }
}
