using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterLoginHistoryFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LoginStudentHistorys_StudentID",
                table: "LoginStudentHistorys",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_LoginStaffHistorys_StaffID",
                table: "LoginStaffHistorys",
                column: "StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginStaffHistorys_Staffs_StaffID",
                table: "LoginStaffHistorys",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoginStudentHistorys_Students_StudentID",
                table: "LoginStudentHistorys",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginStaffHistorys_Staffs_StaffID",
                table: "LoginStaffHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_LoginStudentHistorys_Students_StudentID",
                table: "LoginStudentHistorys");

            migrationBuilder.DropIndex(
                name: "IX_LoginStudentHistorys_StudentID",
                table: "LoginStudentHistorys");

            migrationBuilder.DropIndex(
                name: "IX_LoginStaffHistorys_StaffID",
                table: "LoginStaffHistorys");
        }
    }
}
