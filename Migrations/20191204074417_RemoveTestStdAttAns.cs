using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class RemoveTestStdAttAns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttitudeAnswer",
                table: "TestResultStudentQAnies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttitudeAnswer",
                table: "TestResultStudentQAnies",
                type: "int",
                nullable: true);
        }
    }
}
