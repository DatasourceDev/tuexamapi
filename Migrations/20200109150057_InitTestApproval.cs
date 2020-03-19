using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitTestApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestApprovals",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestID = table.Column<int>(nullable: false),
                    ApprovalCnt = table.Column<int>(nullable: false),
                    ApprovedCnt = table.Column<int>(nullable: false),
                    RejectedCnt = table.Column<int>(nullable: false),
                    StartFrom = table.Column<DateTime>(nullable: false),
                    EndFrom = table.Column<DateTime>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestApprovals", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestApprovals_Tests_TestID",
                        column: x => x.TestID,
                        principalTable: "Tests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestApprovalStaffs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestApprovalID = table.Column<int>(nullable: false),
                    StaffID = table.Column<int>(nullable: false),
                    TestApprovalType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestApprovalStaffs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestApprovalStaffs_TestApprovals_TestApprovalID",
                        column: x => x.TestApprovalID,
                        principalTable: "TestApprovals",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestApprovals_TestID",
                table: "TestApprovals",
                column: "TestID");

            migrationBuilder.CreateIndex(
                name: "IX_TestApprovalStaffs_TestApprovalID",
                table: "TestApprovalStaffs",
                column: "TestApprovalID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestApprovalStaffs");

            migrationBuilder.DropTable(
                name: "TestApprovals");
        }
    }
}
