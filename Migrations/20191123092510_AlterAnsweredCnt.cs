using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterAnsweredCnt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoQuestionCnt",
                table: "TestResultStudents");

            migrationBuilder.AddColumn<int>(
                name: "AnsweredCnt",
                table: "TestResultStudents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnsweredCnt",
                table: "TestResultStudents");

            migrationBuilder.AddColumn<int>(
                name: "DoQuestionCnt",
                table: "TestResultStudents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
