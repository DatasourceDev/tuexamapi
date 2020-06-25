using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterSubjectTypeG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPoint",
                table: "SubjectGSetups");

            migrationBuilder.AddColumn<decimal>(
                name: "Type1Point",
                table: "SubjectGSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Type2Point",
                table: "SubjectGSetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Type3Point",
                table: "SubjectGSetups",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type1Point",
                table: "SubjectGSetups");

            migrationBuilder.DropColumn(
                name: "Type2Point",
                table: "SubjectGSetups");

            migrationBuilder.DropColumn(
                name: "Type3Point",
                table: "SubjectGSetups");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPoint",
                table: "SubjectGSetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
