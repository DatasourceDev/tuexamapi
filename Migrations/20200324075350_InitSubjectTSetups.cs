using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitSubjectTSetups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectTSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescriptionType1 = table.Column<string>(maxLength: 1000, nullable: false),
                    DescriptionType2 = table.Column<string>(maxLength: 1000, nullable: false),
                    DescriptionType3 = table.Column<string>(maxLength: 1000, nullable: false),
                    MaxPoint = table.Column<decimal>(nullable: false),
                    PercentType3 = table.Column<decimal>(nullable: false),
                    PercentType2 = table.Column<decimal>(nullable: false),
                    PercentType1 = table.Column<decimal>(nullable: false),
                    SubjectSubID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTSetups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubjectTSetups_SubjectSubs_SubjectSubID",
                        column: x => x.SubjectSubID,
                        principalTable: "SubjectSubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectTSetups_SubjectSubID",
                table: "SubjectTSetups",
                column: "SubjectSubID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectTSetups");
        }
    }
}
