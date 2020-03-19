using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterSubjectOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "Subjects");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Subjects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Subjects");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Subjects",
                type: "int",
                nullable: true);
        }
    }
}
