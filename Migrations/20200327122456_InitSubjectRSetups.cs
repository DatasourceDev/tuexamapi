using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitSubjectRSetups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectRSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    SubjectSubfromPart1ID = table.Column<int>(nullable: false),
                    Percent = table.Column<decimal>(nullable: false),
                    Sub1MoreThanPercent = table.Column<bool>(nullable: false),
                    Sub2MoreThanPercent = table.Column<bool>(nullable: false),
                    SubjectSubID1 = table.Column<int>(nullable: false),
                    SubjectSubID2 = table.Column<int>(nullable: false),
                    SubjectSub1ID = table.Column<int>(nullable: false),
                    SubjectSub2ID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectRSetups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubjectRSetups_SubjectSubs_SubjectSub1ID",
                        column: x => x.SubjectSub1ID,
                        principalTable: "SubjectSubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectRSetups_SubjectSubs_SubjectSub2ID",
                        column: x => x.SubjectSub2ID,
                        principalTable: "SubjectSubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectRSetups_SubjectSubs_SubjectSubfromPart1ID",
                        column: x => x.SubjectSubfromPart1ID,
                        principalTable: "SubjectSubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRSetups_SubjectSub1ID",
                table: "SubjectRSetups",
                column: "SubjectSub1ID");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRSetups_SubjectSub2ID",
                table: "SubjectRSetups",
                column: "SubjectSub2ID");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRSetups_SubjectSubfromPart1ID",
                table: "SubjectRSetups",
                column: "SubjectSubfromPart1ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectRSetups");
        }
    }
}
