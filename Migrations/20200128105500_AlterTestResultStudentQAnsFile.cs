using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTestResultStudentQAnsFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "TestResultStudentQAnies",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "TestResultStudentQAnies",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "TestResultStudentQAnies",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "TestResultStudentQAnies");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "TestResultStudentQAnies");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "TestResultStudentQAnies");
        }
    }
}
