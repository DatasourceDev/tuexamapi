using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class Initial5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttitudeSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttitudeAnsType = table.Column<int>(nullable: false),
                    Text1 = table.Column<string>(nullable: true),
                    Text2 = table.Column<string>(nullable: true),
                    Text3 = table.Column<string>(nullable: true),
                    Text4 = table.Column<string>(nullable: true),
                    Text5 = table.Column<string>(nullable: true),
                    Text6 = table.Column<string>(nullable: true),
                    Text7 = table.Column<string>(nullable: true),
                    Point = table.Column<decimal>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttitudeSetups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExamSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPeriod = table.Column<int>(nullable: false),
                    choosed = table.Column<bool>(nullable: false),
                    SubjectGroupID = table.Column<int>(nullable: false),
                    SubjectID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSetups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExamSetups_SubjectGroups_SubjectGroupID",
                        column: x => x.SubjectGroupID,
                        principalTable: "SubjectGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSetups_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(nullable: false),
                    Choice = table.Column<string>(nullable: true),
                    QuestionTh = table.Column<string>(nullable: true),
                    QuestionEn = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Point = table.Column<decimal>(nullable: false),
                    FileUrl = table.Column<string>(nullable: true),
                    QuestionID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionAnies_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionChilds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionTh = table.Column<string>(nullable: true),
                    QuestionEn = table.Column<string>(nullable: true),
                    FileUrl = table.Column<string>(nullable: true),
                    QuestionID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionChilds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionChilds_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SendResultSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendByEmail = table.Column<bool>(nullable: false),
                    SendByPost = table.Column<bool>(nullable: false),
                    Other = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    SubjectGroupID = table.Column<int>(nullable: false),
                    SubjectID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendResultSetups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SendResultSetups_SubjectGroups_SubjectGroupID",
                        column: x => x.SubjectGroupID,
                        principalTable: "SubjectGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SendResultSetups_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true),
                    ProveStatus = table.Column<int>(nullable: false),
                    StudentCnt = table.Column<int>(nullable: false),
                    ProvedCnt = table.Column<int>(nullable: false),
                    UnprovedCnt = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResults_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResultStudents",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeCnt = table.Column<int>(nullable: false),
                    QuestionCnt = table.Column<int>(nullable: false),
                    StudentID = table.Column<int>(nullable: false),
                    ExamID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true),
                    Start_On = table.Column<DateTime>(nullable: false),
                    End_On = table.Column<DateTime>(nullable: false),
                    ProveStatus = table.Column<int>(nullable: false),
                    DoQuestionCnt = table.Column<int>(nullable: false),
                    Point = table.Column<int>(nullable: false),
                    CorrectCnt = table.Column<int>(nullable: false),
                    WrongCnt = table.Column<int>(nullable: false),
                    SendByEmail = table.Column<bool>(nullable: false),
                    SendByPost = table.Column<bool>(nullable: false),
                    Other = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultStudents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultStudents_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultStudents_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnsChilds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(nullable: false),
                    Choice = table.Column<string>(nullable: true),
                    QuestionTh = table.Column<string>(nullable: true),
                    QuestionEn = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Point = table.Column<decimal>(nullable: false),
                    FileUrl = table.Column<string>(nullable: true),
                    QuestionChildID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnsChilds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionAnsChilds_QuestionChilds_QuestionChildID",
                        column: x => x.QuestionChildID,
                        principalTable: "QuestionChilds",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResultStudentQAnies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextAnswer = table.Column<string>(nullable: true),
                    FileAnswerUrl = table.Column<string>(nullable: true),
                    AttitudeAnswer = table.Column<int>(nullable: true),
                    TestResultStudentID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false),
                    QuestionAnsID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true),
                    Point = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultStudentQAnies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultStudentQAnies_QuestionAnies_QuestionAnsID",
                        column: x => x.QuestionAnsID,
                        principalTable: "QuestionAnies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultStudentQAnies_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultStudentQAnies_TestResultStudents_TestResultStudentID",
                        column: x => x.TestResultStudentID,
                        principalTable: "TestResultStudents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResultStudentQAnsChilds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextAnswer = table.Column<string>(nullable: true),
                    FileAnswerUrl = table.Column<string>(nullable: true),
                    QuestionChildID = table.Column<int>(nullable: false),
                    QuestionAnsChildID = table.Column<int>(nullable: false),
                    TestResultStudentQAnsID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultStudentQAnsChilds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultStudentQAnsChilds_QuestionAnsChilds_QuestionAnsChildID",
                        column: x => x.QuestionAnsChildID,
                        principalTable: "QuestionAnsChilds",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultStudentQAnsChilds_QuestionChilds_QuestionChildID",
                        column: x => x.QuestionChildID,
                        principalTable: "QuestionChilds",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultStudentQAnsChilds_TestResultStudentQAnies_TestResultStudentQAnsID",
                        column: x => x.TestResultStudentQAnsID,
                        principalTable: "TestResultStudentQAnies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserID",
                table: "Students",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserID",
                table: "Staffs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSetups_SubjectGroupID",
                table: "ExamSetups",
                column: "SubjectGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSetups_SubjectID",
                table: "ExamSetups",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnies_QuestionID",
                table: "QuestionAnies",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnsChilds_QuestionChildID",
                table: "QuestionAnsChilds",
                column: "QuestionChildID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChilds_QuestionID",
                table: "QuestionChilds",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_SendResultSetups_SubjectGroupID",
                table: "SendResultSetups",
                column: "SubjectGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_SendResultSetups_SubjectID",
                table: "SendResultSetups",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_ExamID",
                table: "TestResults",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnies_QuestionAnsID",
                table: "TestResultStudentQAnies",
                column: "QuestionAnsID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnies_QuestionID",
                table: "TestResultStudentQAnies",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnies_TestResultStudentID",
                table: "TestResultStudentQAnies",
                column: "TestResultStudentID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnsChilds_QuestionAnsChildID",
                table: "TestResultStudentQAnsChilds",
                column: "QuestionAnsChildID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnsChilds_QuestionChildID",
                table: "TestResultStudentQAnsChilds",
                column: "QuestionChildID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnsChilds_TestResultStudentQAnsID",
                table: "TestResultStudentQAnsChilds",
                column: "TestResultStudentQAnsID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudents_ExamID",
                table: "TestResultStudents",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudents_StudentID",
                table: "TestResultStudents",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Users_UserID",
                table: "Staffs",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UserID",
                table: "Students",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Users_UserID",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UserID",
                table: "Students");

            migrationBuilder.DropTable(
                name: "AttitudeSetups");

            migrationBuilder.DropTable(
                name: "ExamSetups");

            migrationBuilder.DropTable(
                name: "SendResultSetups");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "TestResultStudentQAnsChilds");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "QuestionAnsChilds");

            migrationBuilder.DropTable(
                name: "TestResultStudentQAnies");

            migrationBuilder.DropTable(
                name: "QuestionChilds");

            migrationBuilder.DropTable(
                name: "QuestionAnies");

            migrationBuilder.DropTable(
                name: "TestResultStudents");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_UserID",
                table: "Staffs");
        }
    }
}
