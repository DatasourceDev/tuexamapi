using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class RemoveQuestionChild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResultStudentQAnsChilds_QuestionAnsChilds_QuestionAnsChildID",
                table: "TestResultStudentQAnsChilds");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResultStudentQAnsChilds_QuestionChilds_QuestionChildID",
                table: "TestResultStudentQAnsChilds");

            migrationBuilder.DropTable(
                name: "QuestionAnsChilds");

            migrationBuilder.DropTable(
                name: "QuestionChilds");

            migrationBuilder.DropIndex(
                name: "IX_TestResultStudentQAnsChilds_QuestionAnsChildID",
                table: "TestResultStudentQAnsChilds");

            migrationBuilder.DropIndex(
                name: "IX_TestResultStudentQAnsChilds_QuestionChildID",
                table: "TestResultStudentQAnsChilds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionChilds",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Create_By = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    QuestionTh = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Update_By = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "QuestionAnsChilds",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Choice = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Create_By = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuestionChildID = table.Column<int>(type: "int", nullable: false),
                    QuestionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QuestionTh = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Update_By = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnsChilds_QuestionAnsChildID",
                table: "TestResultStudentQAnsChilds",
                column: "QuestionAnsChildID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultStudentQAnsChilds_QuestionChildID",
                table: "TestResultStudentQAnsChilds",
                column: "QuestionChildID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnsChilds_QuestionChildID",
                table: "QuestionAnsChilds",
                column: "QuestionChildID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChilds_QuestionID",
                table: "QuestionChilds",
                column: "QuestionID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultStudentQAnsChilds_QuestionAnsChilds_QuestionAnsChildID",
                table: "TestResultStudentQAnsChilds",
                column: "QuestionAnsChildID",
                principalTable: "QuestionAnsChilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultStudentQAnsChilds_QuestionChilds_QuestionChildID",
                table: "TestResultStudentQAnsChilds",
                column: "QuestionChildID",
                principalTable: "QuestionChilds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
