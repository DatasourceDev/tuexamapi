using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterQuestionFilename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Questions",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Questions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
