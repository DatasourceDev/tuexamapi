using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterAnswerSubjectSub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AnswerSubjectSub1",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnswerSubjectSub2",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnswerSubjectSub3",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnswerSubjectSub4",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnswerSubjectSub5",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnswerSubjectSub6",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnswerSubjectSub7",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerSubjectSub1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerSubjectSub2",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerSubjectSub3",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerSubjectSub4",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerSubjectSub5",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerSubjectSub6",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerSubjectSub7",
                table: "Questions");
        }
    }
}
