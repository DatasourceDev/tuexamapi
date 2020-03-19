using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterAttitudeSetupPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Point",
                table: "AttitudeSetups");

            migrationBuilder.AddColumn<decimal>(
                name: "Point1",
                table: "AttitudeSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Point2",
                table: "AttitudeSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Point3",
                table: "AttitudeSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Point4",
                table: "AttitudeSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Point5",
                table: "AttitudeSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Point6",
                table: "AttitudeSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Point7",
                table: "AttitudeSetups",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Point1",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "Point2",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "Point3",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "Point4",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "Point5",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "Point6",
                table: "AttitudeSetups");

            migrationBuilder.DropColumn(
                name: "Point7",
                table: "AttitudeSetups");

            migrationBuilder.AddColumn<decimal>(
                name: "Point",
                table: "AttitudeSetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
