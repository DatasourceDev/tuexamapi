using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterFileAnswerUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileAnswerUrl",
                table: "TestResultStudentQAnies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileAnswerUrl",
                table: "TestResultStudentQAnies",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
