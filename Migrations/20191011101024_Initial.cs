using Microsoft.EntityFrameworkCore.Migrations;

namespace tuexamapi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentCode = table.Column<string>(nullable: false),
                    Prefix = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 250, nullable: true),
                    LastName = table.Column<string>(maxLength: 250, nullable: true),
                    FirstNameEn = table.Column<string>(maxLength: 250, nullable: true),
                    LastNameEn = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IDCard = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Passport = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Faculty = table.Column<string>(nullable: true),
                    Course = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubjectGroups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SubjectGroupID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Subjects_SubjectGroups_SubjectGroupID",
                        column: x => x.SubjectGroupID,
                        principalTable: "SubjectGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectSubs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SubjectID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectSubs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubjectSubs_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionCode = table.Column<string>(maxLength: 250, nullable: false),
                    QuestionType = table.Column<int>(nullable: false),
                    Course = table.Column<int>(nullable: false),
                    QuestionTh = table.Column<string>(nullable: true),
                    QuestionEn = table.Column<string>(nullable: true),
                    FileUrl = table.Column<string>(nullable: true),
                    TimeLimit = table.Column<int>(nullable: false),
                    TimeLimitType = table.Column<int>(nullable: false),
                    Keyword = table.Column<string>(nullable: true),
                    QuestionLevel = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ApprovalStatus = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    AttitudeAnsType = table.Column<int>(nullable: true),
                    SubjectGroupID = table.Column<int>(nullable: false),
                    SubjectID = table.Column<int>(nullable: false),
                    SubjectSubID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Questions_SubjectGroups_SubjectGroupID",
                        column: x => x.SubjectGroupID,
                        principalTable: "SubjectGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_SubjectSubs_SubjectSubID",
                        column: x => x.SubjectSubID,
                        principalTable: "SubjectSubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubjectGroupID",
                table: "Questions",
                column: "SubjectGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubjectID",
                table: "Questions",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubjectSubID",
                table: "Questions",
                column: "SubjectSubID");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectGroupID",
                table: "Subjects",
                column: "SubjectGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectSubs_SubjectID",
                table: "SubjectSubs",
                column: "SubjectID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "SubjectSubs");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectGroups");
        }
    }
}
