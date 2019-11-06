using Microsoft.EntityFrameworkCore.Migrations;

namespace NoSocNet.Infrastructure.Data.Migrations
{
    public partial class add_index_messages_sendDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_SendDate",
                table: "Messages",
                column: "SendDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_SendDate",
                table: "Messages");
        }
    }
}
