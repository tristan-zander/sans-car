using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class QuoteAndGuildModelBuilder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GuildId1",
                table: "Quotes",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_GuildId1",
                table: "Quotes",
                column: "GuildId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Guilds_GuildId1",
                table: "Quotes",
                column: "GuildId1",
                principalTable: "Guilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Guilds_GuildId1",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_GuildId1",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "GuildId1",
                table: "Quotes");
        }
    }
}
