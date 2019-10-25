using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "SubjectSubs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "SubjectSubs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "SubjectSubs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "SubjectSubs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "Subjects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "Subjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "Subjects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "Subjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "SubjectGroups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "SubjectGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "SubjectGroups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "SubjectGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "Staffs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "Staffs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "Staffs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "Staffs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "Exams",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "Exams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "Exams",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "Exams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "ExamRegisters",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "ExamRegisters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "ExamRegisters",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "ExamRegisters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "SubjectSubs");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "SubjectSubs");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "SubjectSubs");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "SubjectSubs");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "ExamRegisters");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "ExamRegisters");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "ExamRegisters");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "ExamRegisters");
        }
    }
}
