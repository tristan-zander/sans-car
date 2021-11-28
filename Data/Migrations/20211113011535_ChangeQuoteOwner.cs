using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ChangeQuoteOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_DiscordUser_OwnerId",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_OwnerId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Quotes");

            migrationBuilder.AddColumn<decimal>(
                name: "Owner",
                table: "Quotes",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Quotes");

            migrationBuilder.AddColumn<decimal>(
                name: "OwnerId",
                table: "Quotes",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_OwnerId",
                table: "Quotes",
                column: "OwnerId");

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
