using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterSubjectESetupSubjectSub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectSubID",
                table: "SubjectESetups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectESetups_SubjectSubID",
                table: "SubjectESetups",
                column: "SubjectSubID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectESetups_SubjectSubs_SubjectSubID",
                table: "SubjectESetups",
                column: "SubjectSubID",
                principalTable: "SubjectSubs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectESetups_SubjectSubs_SubjectSubID",
                table: "SubjectESetups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectESetups_SubjectSubID",
                table: "SubjectESetups");

            migrationBuilder.DropColumn(
                name: "SubjectSubID",
                table: "SubjectESetups");
        }
    }
}
