using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class UpdateTo60 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuoteId",
                table: "Quote",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "HasAgreedToToS",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAgreedToToS",
                table: "Guilds");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Quote",
                newName: "QuoteId");
        }
    }
}
