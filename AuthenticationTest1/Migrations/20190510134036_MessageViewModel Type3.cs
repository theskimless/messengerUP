using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthenticationTest1.Migrations
{
    public partial class MessageViewModelType3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
