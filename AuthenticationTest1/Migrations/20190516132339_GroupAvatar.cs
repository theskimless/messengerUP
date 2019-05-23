using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthenticationTest1.Migrations
{
    public partial class GroupAvatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Groups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Groups");
        }
    }
}
