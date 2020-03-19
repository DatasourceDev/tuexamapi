using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitQuestionApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionApprovals",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionID = table.Column<int>(nullable: false),
                    ApprovalCnt = table.Column<int>(nullable: false),
                    ApprovedCnt = table.Column<int>(nullable: false),
                    RejectedCnt = table.Column<int>(nullable: false),
                    StartFrom = table.Column<DateTime>(nullable: false),
                    EndFrom = table.Column<DateTime>(nullable: false),
                    QuestionApprovalType = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionApprovals", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionApprovals_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionApprovalStaffs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionApprovalID = table.Column<int>(nullable: false),
                    StaffID = table.Column<int>(nullable: false),
                    QuestionApprovalType = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(maxLength: 1000, nullable: true),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionApprovalStaffs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionApprovalStaffs_QuestionApprovals_QuestionApprovalID",
                        column: x => x.QuestionApprovalID,
                        principalTable: "QuestionApprovals",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionApprovalStaffs_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionApprovals_QuestionID",
                table: "QuestionApprovals",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionApprovalStaffs_QuestionApprovalID",
                table: "QuestionApprovalStaffs",
                column: "QuestionApprovalID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionApprovalStaffs_StaffID",
                table: "QuestionApprovalStaffs",
                column: "StaffID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionApprovalStaffs");

            migrationBuilder.DropTable(
                name: "QuestionApprovals");
        }
    }
}
