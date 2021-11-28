using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class UpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Users_OwnerId",
                table: "Quotes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddColumn<string>(
                name: "OwnerAccountId",
                table: "Quotes",
                type: "text",
                nullable: true);

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
                name: "IX_Quotes_OwnerAccountId",
                table: "Quotes",
                column: "OwnerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordUser_QuoteId",
                table: "DiscordUser",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_AspNetUsers_OwnerAccountId",
                table: "Quotes",
                column: "OwnerAccountId",
                principalTable: "AspNetUsers",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_AspNetUsers_OwnerAccountId",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_DiscordUser_OwnerId",
                table: "Quotes");

            migrationBuilder.DropTable(
                name: "DiscordUser");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_OwnerAccountId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "OwnerAccountId",
                table: "Quotes");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_QuoteId",
                table: "Users",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Users_OwnerId",
                table: "Quotes",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
