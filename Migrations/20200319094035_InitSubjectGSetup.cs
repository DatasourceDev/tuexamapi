using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitSubjectGSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectGSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: false),
                    PercentByType = table.Column<decimal>(nullable: false),
                    PercentBySubjectSub = table.Column<decimal>(nullable: false),
                    MaxPoint = table.Column<decimal>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGSetups", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectGSetups");
        }
    }
}
