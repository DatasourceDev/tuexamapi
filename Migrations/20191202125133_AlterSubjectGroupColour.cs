using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterSubjectGroupColour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color1",
                table: "SubjectGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color2",
                table: "SubjectGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color3",
                table: "SubjectGroups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color1",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "Color2",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "Color3",
                table: "SubjectGroups");
        }
    }
}
