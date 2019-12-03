using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterExamRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamCode",
                table: "ExamRegisters");

            migrationBuilder.DropColumn(
                name: "ExamDate",
                table: "ExamRegisters");

            migrationBuilder.DropColumn(
                name: "ExamPeriod",
                table: "ExamRegisters");

            migrationBuilder.DropColumn(
                name: "ExamRegisterType",
                table: "ExamRegisters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExamRegisters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExamCode",
                table: "ExamRegisters",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExamDate",
                table: "ExamRegisters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ExamPeriod",
                table: "ExamRegisters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExamRegisterType",
                table: "ExamRegisters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ExamRegisters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
