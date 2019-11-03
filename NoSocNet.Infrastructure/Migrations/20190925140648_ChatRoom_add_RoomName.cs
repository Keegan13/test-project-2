using Microsoft.EntityFrameworkCore.Migrations;

namespace NoSocNet.DAL.Migrations
{
    public partial class ChatRoom_add_RoomName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "ChatRooms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "ChatRooms");
        }
    }
}
