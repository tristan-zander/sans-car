using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class DiscordUsersToUlongHashSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_DiscordUsers_OwnerId",
                table: "Quotes");

            migrationBuilder.DropTable(
                name: "DiscordUsers");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_OwnerId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Quotes");

            migrationBuilder.AddColumn<decimal[]>(
                name: "Mentions",
                table: "Quotes",
                type: "numeric[]",
                nullable: true);

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
                name: "Mentions",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Quotes");

            migrationBuilder.AddColumn<decimal>(
                name: "OwnerId",
                table: "Quotes",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiscordUsers",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordUsers_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_OwnerId",
                table: "Quotes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordUsers_QuoteId",
                table: "DiscordUsers",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_DiscordUsers_OwnerId",
                table: "Quotes",
                column: "OwnerId",
                principalTable: "DiscordUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
