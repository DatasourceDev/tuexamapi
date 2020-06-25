using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterSubjectESetupFromTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentHigh",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentLow",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentMid",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType1",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType2",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType3",
                table: "SubjectESetups");

            migrationBuilder.AddColumn<decimal>(
                name: "PercentHighfrom",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentHighto",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentLowfrom",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentLowto",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentMidfrom",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentMidto",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType1from",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType1to",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType2from",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType2to",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType3from",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType3to",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentHighfrom",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentHighto",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentLowfrom",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentLowto",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentMidfrom",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentMidto",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType1from",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType1to",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType2from",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType2to",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType3from",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "PercentType3to",
                table: "SubjectESetups");

            migrationBuilder.AddColumn<decimal>(
                name: "PercentHigh",
                table: "SubjectESetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentLow",
                table: "SubjectESetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentMid",
                table: "SubjectESetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType1",
                table: "SubjectESetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType2",
                table: "SubjectESetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentType3",
                table: "SubjectESetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
