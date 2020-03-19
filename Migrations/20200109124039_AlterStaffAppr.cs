using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterStaffAppr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAdmin",
                table: "Staffs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isMasterAdmin",
                table: "Staffs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isMasterQuestionAppr",
                table: "Staffs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isMasterTestAppr",
                table: "Staffs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isQuestionAppr",
                table: "Staffs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTestAppr",
                table: "Staffs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAdmin",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "isMasterAdmin",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "isMasterQuestionAppr",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "isMasterTestAppr",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "isQuestionAppr",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "isTestAppr",
                table: "Staffs");
        }
    }
}
