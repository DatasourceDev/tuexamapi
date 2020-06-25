using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class AlterSubjectRSetups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectRSetups_SubjectSubs_SubjectSub1ID",
                table: "SubjectRSetups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectRSetups_SubjectSubs_SubjectSub2ID",
                table: "SubjectRSetups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectRSetups_SubjectSubs_SubjectSubfromPart1ID",
                table: "SubjectRSetups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectRSetups_SubjectSub1ID",
                table: "SubjectRSetups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectRSetups_SubjectSub2ID",
                table: "SubjectRSetups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectRSetups_SubjectSubfromPart1ID",
                table: "SubjectRSetups");

            migrationBuilder.DropColumn(
                name: "SubjectSub1ID",
                table: "SubjectRSetups");

            migrationBuilder.DropColumn(
                name: "SubjectSub2ID",
                table: "SubjectRSetups");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectSubfromPart1ID",
                table: "SubjectRSetups",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubjectSubfromPart1ID",
                table: "SubjectRSetups",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubjectSub1ID",
                table: "SubjectRSetups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectSub2ID",
                table: "SubjectRSetups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRSetups_SubjectSub1ID",
                table: "SubjectRSetups",
                column: "SubjectSub1ID");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRSetups_SubjectSub2ID",
                table: "SubjectRSetups",
                column: "SubjectSub2ID");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRSetups_SubjectSubfromPart1ID",
                table: "SubjectRSetups",
                column: "SubjectSubfromPart1ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectRSetups_SubjectSubs_SubjectSub1ID",
                table: "SubjectRSetups",
                column: "SubjectSub1ID",
                principalTable: "SubjectSubs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectRSetups_SubjectSubs_SubjectSub2ID",
                table: "SubjectRSetups",
                column: "SubjectSub2ID",
                principalTable: "SubjectSubs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectRSetups_SubjectSubs_SubjectSubfromPart1ID",
                table: "SubjectRSetups",
                column: "SubjectSubfromPart1ID",
                principalTable: "SubjectSubs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
