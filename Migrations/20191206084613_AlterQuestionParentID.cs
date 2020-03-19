using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterQuestionParentID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionID",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "ChildOrder",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildOrder",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "QuestionID",
                table: "Questions",
                type: "int",
                nullable: true);
        }
    }
}
