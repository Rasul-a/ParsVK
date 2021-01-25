using Microsoft.EntityFrameworkCore.Migrations;

namespace ParsVK.Migrations
{
    public partial class AddGroupCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembersCount",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Profiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembersCount",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Profiles");
        }
    }
}
