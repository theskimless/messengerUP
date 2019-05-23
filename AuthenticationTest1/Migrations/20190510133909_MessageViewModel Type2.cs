using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthenticationTest1.Migrations
{
    public partial class MessageViewModelType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Messages");
        }
    }
}
