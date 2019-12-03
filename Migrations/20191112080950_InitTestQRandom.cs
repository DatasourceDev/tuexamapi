using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitTestQRandom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestQRandoms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionType = table.Column<int>(nullable: false),
                    TestID = table.Column<int>(nullable: false),
                    SubjectSubID = table.Column<int>(nullable: false),
                    VeryEasy = table.Column<int>(nullable: true),
                    Easy = table.Column<int>(nullable: true),
                    Mid = table.Column<int>(nullable: true),
                    Hard = table.Column<int>(nullable: true),
                    VeryHard = table.Column<int>(nullable: true),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestQRandoms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestQRandoms_SubjectSubs_SubjectSubID",
                        column: x => x.SubjectSubID,
                        principalTable: "SubjectSubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestQRandoms_Tests_TestID",
                        column: x => x.TestID,
                        principalTable: "Tests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestQRandoms_SubjectSubID",
                table: "TestQRandoms",
                column: "SubjectSubID");

            migrationBuilder.CreateIndex(
                name: "IX_TestQRandoms_TestID",
                table: "TestQRandoms",
                column: "TestID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestQRandoms");
        }
    }
}
