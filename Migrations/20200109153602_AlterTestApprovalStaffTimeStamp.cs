using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTestApprovalStaffTimeStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "TestApprovalStaffs",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "TestApprovalStaffs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "TestApprovalStaffs",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "TestApprovalStaffs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "TestApprovalStaffs");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "TestApprovalStaffs");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "TestApprovalStaffs");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "TestApprovalStaffs");
        }
    }
}
