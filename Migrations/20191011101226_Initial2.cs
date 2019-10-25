using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffCode = table.Column<string>(nullable: false),
                    Prefix = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 250, nullable: true),
                    LastName = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IDCard = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Passport = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    OpenDate = table.Column<DateTime>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCode = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TestQuestionType = table.Column<int>(nullable: false),
                    TestCustomOrderType = table.Column<int>(nullable: false),
                    TimeLimit = table.Column<int>(nullable: false),
                    TimeLimitType = table.Column<int>(nullable: false),
                    TestDoExamType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ApprovalStatus = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Course = table.Column<int>(nullable: false),
                    ShowResult = table.Column<bool>(nullable: false),
                    SubjectGroupID = table.Column<int>(nullable: false),
                    SubjectID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tests_SubjectGroups_SubjectGroupID",
                        column: x => x.SubjectGroupID,
                        principalTable: "SubjectGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tests_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamCode = table.Column<string>(maxLength: 250, nullable: false),
                    ExamDate = table.Column<DateTime>(nullable: false),
                    ExamPeriod = table.Column<int>(nullable: false),
                    ExamTestType = table.Column<int>(nullable: false),
                    SubjectGroupID = table.Column<int>(nullable: false),
                    SubjectID = table.Column<int>(nullable: false),
                    TestID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Exams_SubjectGroups_SubjectGroupID",
                        column: x => x.SubjectGroupID,
                        principalTable: "SubjectGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_Tests_TestID",
                        column: x => x.TestID,
                        principalTable: "Tests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamRegisters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamCode = table.Column<string>(maxLength: 250, nullable: false),
                    ExamDate = table.Column<DateTime>(nullable: false),
                    ExamPeriod = table.Column<int>(nullable: false),
                    ExamRegisterType = table.Column<int>(nullable: false),
                    ExamID = table.Column<int>(nullable: false),
                    StudentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamRegisters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExamRegisters_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamRegisters_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamRegisters_ExamID",
                table: "ExamRegisters",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRegisters_StudentID",
                table: "ExamRegisters",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SubjectGroupID",
                table: "Exams",
                column: "SubjectGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SubjectID",
                table: "Exams",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_TestID",
                table: "Exams",
                column: "TestID");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_SubjectGroupID",
                table: "Tests",
                column: "SubjectGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_SubjectID",
                table: "Tests",
                column: "SubjectID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamRegisters");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Tests");
        }
    }
}
