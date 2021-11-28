using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddDiscordUserTableToContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordUser_Quotes_QuoteId",
                table: "DiscordUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_DiscordUser_OwnerId",
                table: "Quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscordUser",
                table: "DiscordUser");

            migrationBuilder.RenameTable(
                name: "DiscordUser",
                newName: "DiscordUsers");

            migrationBuilder.RenameIndex(
                name: "IX_DiscordUser_QuoteId",
                table: "DiscordUsers",
                newName: "IX_DiscordUsers_QuoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscordUsers",
                table: "DiscordUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordUsers_Quotes_QuoteId",
                table: "DiscordUsers",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_DiscordUsers_OwnerId",
                table: "Quotes",
                column: "OwnerId",
                principalTable: "DiscordUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordUsers_Quotes_QuoteId",
                table: "DiscordUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_DiscordUsers_OwnerId",
                table: "Quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscordUsers",
                table: "DiscordUsers");

            migrationBuilder.RenameTable(
                name: "DiscordUsers",
                newName: "DiscordUser");

            migrationBuilder.RenameIndex(
                name: "IX_DiscordUsers_QuoteId",
                table: "DiscordUser",
                newName: "IX_DiscordUser_QuoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscordUser",
                table: "DiscordUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordUser_Quotes_QuoteId",
                table: "DiscordUser",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_DiscordUser_OwnerId",
                table: "Quotes",
                column: "OwnerId",
                principalTable: "DiscordUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
