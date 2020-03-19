using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class RemoveTestResultStudentQAnsChoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Choice",
                table: "TestResultStudentQAnies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Choice",
                table: "TestResultStudentQAnies",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
