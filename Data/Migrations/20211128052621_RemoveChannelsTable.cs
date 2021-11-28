using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class RemoveChannelsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_AudioPlayer_AudioPlayerId",
                table: "Guilds");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Channels_QuoteChannelId",
                table: "Guilds");

            migrationBuilder.DropTable(
                name: "AudioPlayer");

            migrationBuilder.DropTable(
                name: "Song");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "SongQueue");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_AudioPlayerId",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_QuoteChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "AllowAudio",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "AudioPlayerId",
                table: "Guilds");

            migrationBuilder.RenameColumn(
                name: "QuoteChannelId",
                table: "Guilds",
                newName: "QuoteChannel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuoteChannel",
                table: "Guilds",
                newName: "QuoteChannelId");

            migrationBuilder.AddColumn<bool>(
                name: "AllowAudio",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AudioPlayerId",
                table: "Guilds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SongQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AudioPlayer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastActiveTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SongQueueId = table.Column<Guid>(type: "uuid", nullable: true),
                    VoiceChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioPlayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioPlayer_Channels_VoiceChannelId",
                        column: x => x.VoiceChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AudioPlayer_SongQueue_SongQueueId",
                        column: x => x.SongQueueId,
                        principalTable: "SongQueue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Song",
                columns: table => new
                {
                    SongId = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Length = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SongQueueId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Song", x => x.SongId);
                    table.ForeignKey(
                        name: "FK_Song_SongQueue_SongQueueId",
                        column: x => x.SongQueueId,
                        principalTable: "SongQueue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_AudioPlayerId",
                table: "Guilds",
                column: "AudioPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_QuoteChannelId",
                table: "Guilds",
                column: "QuoteChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioPlayer_SongQueueId",
                table: "AudioPlayer",
                column: "SongQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioPlayer_VoiceChannelId",
                table: "AudioPlayer",
                column: "VoiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Song_SongQueueId",
                table: "Song",
                column: "SongQueueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_AudioPlayer_AudioPlayerId",
                table: "Guilds",
                column: "AudioPlayerId",
                principalTable: "AudioPlayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Channels_QuoteChannelId",
                table: "Guilds",
                column: "QuoteChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
