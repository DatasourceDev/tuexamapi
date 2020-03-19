using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterTestApprovalStaff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TestApprovalStaffs_StaffID",
                table: "TestApprovalStaffs",
                column: "StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestApprovalStaffs_Staffs_StaffID",
                table: "TestApprovalStaffs",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestApprovalStaffs_Staffs_StaffID",
                table: "TestApprovalStaffs");

            migrationBuilder.DropIndex(
                name: "IX_TestApprovalStaffs_StaffID",
                table: "TestApprovalStaffs");
        }
    }
}
