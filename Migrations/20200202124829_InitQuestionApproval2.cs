using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class InitQuestionApproval2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionApprovalType",
                table: "QuestionApprovals");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "QuestionApprovals",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "QuestionApprovals");

            migrationBuilder.AddColumn<int>(
                name: "QuestionApprovalType",
                table: "QuestionApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
