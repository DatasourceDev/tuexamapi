using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTestResultStudentExpriryOn2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "แ",
                table: "TestResultStudents");

            migrationBuilder.AddColumn<DateTime>(
                name: "Expriry_On",
                table: "TestResultStudents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expriry_On",
                table: "TestResultStudents");

            migrationBuilder.AddColumn<DateTime>(
                name: "แ",
                table: "TestResultStudents",
                type: "datetime2",
                nullable: true);
        }
    }
}
