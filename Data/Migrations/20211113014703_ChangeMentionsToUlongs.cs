using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ChangeMentionsToUlongs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordUser");

            migrationBuilder.AddColumn<List<ulong>>(
                name: "Mentions",
                table: "Quotes",
                type: "numeric(20,0)[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mentions",
                table: "Quotes");

            migrationBuilder.CreateTable(
                name: "DiscordUser",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordUser_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscordUser_QuoteId",
                table: "DiscordUser",
                column: "QuoteId");
        }
    }
}
