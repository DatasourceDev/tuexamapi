using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterSubjectGSetupDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SubjectGSetups");

            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "SubjectGSetups");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionType1",
                table: "SubjectGSetups",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionType2",
                table: "SubjectGSetups",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionType3",
                table: "SubjectGSetups",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionType1",
                table: "SubjectGSetups");

            migrationBuilder.DropColumn(
                name: "DescriptionType2",
                table: "SubjectGSetups");

            migrationBuilder.DropColumn(
                name: "DescriptionType3",
                table: "SubjectGSetups");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SubjectGSetups",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SubjectType",
                table: "SubjectGSetups",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
