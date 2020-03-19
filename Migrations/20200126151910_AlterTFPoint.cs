using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTFPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FPoint",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TPoint",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FPoint",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TPoint",
                table: "Questions");
        }
    }
}
