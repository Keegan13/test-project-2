using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NoSocNet.Infrastructure.Data.Migrations
{
    public partial class FK_rename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_AspNetUsers_OwnerId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "SenderUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                newName: "IX_Messages_SenderUserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "ChatRooms",
                newName: "OwnerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRooms_OwnerId",
                table: "ChatRooms",
                newName: "IX_ChatRooms_OwnerUserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRead",
                table: "MessagesReadBy",
                nullable: true,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getdate()");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_AspNetUsers_OwnerUserId",
                table: "ChatRooms",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderUserId",
                table: "Messages",
                column: "SenderUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_AspNetUsers_OwnerUserId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderUserId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderUserId",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderUserId",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.RenameColumn(
                name: "OwnerUserId",
                table: "ChatRooms",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRooms_OwnerUserId",
                table: "ChatRooms",
                newName: "IX_ChatRooms_OwnerId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRead",
                table: "MessagesReadBy",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValueSql: "getdate()");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_AspNetUsers_OwnerId",
                table: "ChatRooms",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
