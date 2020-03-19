using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterQuestionPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Point1",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point2",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point3",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point4",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point5",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point6",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point7",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Point1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Point2",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Point3",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Point4",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Point5",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Point6",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Point7",
                table: "Questions");
        }
    }
}
