using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterQuestionParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionID",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionParentID",
                table: "Questions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionParentID",
                table: "Questions",
                column: "QuestionParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Questions_QuestionParentID",
                table: "Questions",
                column: "QuestionParentID",
                principalTable: "Questions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Questions_QuestionParentID",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionParentID",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionID",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionParentID",
                table: "Questions");
        }
    }
}
