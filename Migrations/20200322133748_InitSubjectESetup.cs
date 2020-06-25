using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitSubjectESetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectESetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescriptionType1 = table.Column<string>(maxLength: 1000, nullable: false),
                    DescriptionType2 = table.Column<string>(maxLength: 1000, nullable: false),
                    DescriptionType3 = table.Column<string>(maxLength: 1000, nullable: false),
                    PercentType3 = table.Column<decimal>(nullable: false),
                    PercentType2 = table.Column<decimal>(nullable: false),
                    PercentType1 = table.Column<decimal>(nullable: false),
                    PercentHigh = table.Column<decimal>(nullable: false),
                    PercentMid = table.Column<decimal>(nullable: false),
                    PercentLow = table.Column<decimal>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectESetups", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectESetups");
        }
    }
}
