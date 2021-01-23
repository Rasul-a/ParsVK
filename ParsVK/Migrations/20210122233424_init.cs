using Microsoft.EntityFrameworkCore.Migrations;

namespace ParsVK.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Bdate = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    Audios = table.Column<int>(nullable: false),
                    Photos = table.Column<int>(nullable: false),
                    Friends = table.Column<int>(nullable: false),
                    Groups = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LikeUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<string>(nullable: true),
                    ProfileId = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    LikeCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikeUsers_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WallItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CommentsCount = table.Column<int>(nullable: false),
                    LikesCount = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    HistoryText = table.Column<string>(nullable: true),
                    HistoryId = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ProfileId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WallItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WallItems_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeUsers_ProfileId",
                table: "LikeUsers",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WallItems_ProfileId",
                table: "WallItems",
                column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikeUsers");

            migrationBuilder.DropTable(
                name: "WallItems");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
