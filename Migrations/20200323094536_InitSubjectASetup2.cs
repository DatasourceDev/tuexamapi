using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitSubjectASetup2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectASetups",
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
                    table.PrimaryKey("PK_SubjectASetups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubjectASetups_SubjectSubs_SubjectSubID",
                        column: x => x.SubjectSubID,
                        principalTable: "SubjectSubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectASetups_SubjectSubID",
                table: "SubjectASetups",
                column: "SubjectSubID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectASetups");
        }
    }
}
