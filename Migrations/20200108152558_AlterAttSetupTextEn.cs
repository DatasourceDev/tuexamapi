using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterAttSetupTextEn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextEn1",
                table: "AttitudeSetups",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn2",
                table: "AttitudeSetups",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn3",
                table: "AttitudeSetups",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn4",
                table: "AttitudeSetups",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn5",
                table: "AttitudeSetups",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn6",
                table: "AttitudeSetups",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn7",
                table: "AttitudeSetups",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextEn1",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "TextEn2",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "TextEn3",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "TextEn4",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "TextEn5",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "TextEn6",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "TextEn7",
                table: "AttitudeSetups");
        }
    }
}
