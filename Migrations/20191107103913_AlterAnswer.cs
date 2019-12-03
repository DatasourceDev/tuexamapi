using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionEn",
                table: "QuestionAnies");

            migrationBuilder.DropColumn(
                name: "QuestionTh",
                table: "QuestionAnies");

            migrationBuilder.AddColumn<string>(
                name: "AnswerEn",
                table: "QuestionAnies",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerTh",
                table: "QuestionAnies",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerEn",
                table: "QuestionAnies");

            migrationBuilder.DropColumn(
                name: "AnswerTh",
                table: "QuestionAnies");

            migrationBuilder.AddColumn<string>(
                name: "QuestionEn",
                table: "QuestionAnies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionTh",
                table: "QuestionAnies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
