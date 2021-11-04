using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Title = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Length = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    SongQueueId = table.Column<Guid>(type: "uuid", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    AllowSearchCommands = table.Column<bool>(type: "boolean", nullable: false),
                    AllowQuotes = table.Column<bool>(type: "boolean", nullable: false),
                    EnableQuoteChannel = table.Column<bool>(type: "boolean", nullable: false),
                    QuoteChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    AllowAudio = table.Column<bool>(type: "boolean", nullable: false),
                    AudioPlayerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_Guilds_AudioPlayer_AudioPlayerId",
                        column: x => x.AudioPlayerId,
                        principalTable: "AudioPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_Channels_QuoteChannelId",
                        column: x => x.QuoteChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quote",
                columns: table => new
                {
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Message = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    TimeAdded = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PreviouslyModifiedQuotes = table.Column<List<string>>(type: "text[]", nullable: true),
                    OwnerId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    DiscordMessage = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quote", x => x.QuoteId);
                    table.ForeignKey(
                        name: "FK_Quote_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_Users_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "QuoteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudioPlayer_SongQueueId",
                table: "AudioPlayer",
                column: "SongQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioPlayer_VoiceChannelId",
                table: "AudioPlayer",
                column: "VoiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_AudioPlayerId",
                table: "Guilds",
                column: "AudioPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_QuoteChannelId",
                table: "Guilds",
                column: "QuoteChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_GuildId",
                table: "Quote",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_OwnerId",
                table: "Quote",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Song_SongQueueId",
                table: "Song",
                column: "SongQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_QuoteId",
                table: "Users",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_Users_OwnerId",
                table: "Quote",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioPlayer_Channels_VoiceChannelId",
                table: "AudioPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Channels_QuoteChannelId",
                table: "Guilds");

            migrationBuilder.DropForeignKey(
                name: "FK_AudioPlayer_SongQueue_SongQueueId",
                table: "AudioPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_AudioPlayer_AudioPlayerId",
                table: "Guilds");

            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Guilds_GuildId",
                table: "Quote");

            migrationBuilder.DropForeignKey(
                name: "FK_Quote_Users_OwnerId",
                table: "Quote");

            migrationBuilder.DropTable(
                name: "Song");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "SongQueue");

            migrationBuilder.DropTable(
                name: "AudioPlayer");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Quote");
        }
    }
}
