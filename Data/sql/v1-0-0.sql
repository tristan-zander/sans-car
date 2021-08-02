CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE TABLE "Channels" (
        "Id" numeric(20,0) NOT NULL,
        CONSTRAINT "PK_Channels" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE TABLE "SongQueue" (
        "Id" uuid NOT NULL,
        CONSTRAINT "PK_SongQueue" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE TABLE "Users" (
        "Id" numeric(20,0) NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE TABLE "AudioPlayer" (
        "Id" uuid NOT NULL,
        "IsActive" boolean NOT NULL,
        "LastActiveTime" timestamp without time zone NOT NULL,
        "SongQueueId" uuid NULL,
        "VoiceChannelId" numeric(20,0) NULL,
        CONSTRAINT "PK_AudioPlayer" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AudioPlayer_Channels_VoiceChannelId" FOREIGN KEY ("VoiceChannelId") REFERENCES "Channels" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_AudioPlayer_SongQueue_SongQueueId" FOREIGN KEY ("SongQueueId") REFERENCES "SongQueue" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE TABLE "Song" (
        "SongId" text NOT NULL,
        "Title" text NOT NULL,
        "Author" text NOT NULL,
        "Length" interval NOT NULL,
        "Uri" text NOT NULL,
        "SongQueueId" uuid NULL,
        CONSTRAINT "PK_Song" PRIMARY KEY ("SongId"),
        CONSTRAINT "FK_Song_SongQueue_SongQueueId" FOREIGN KEY ("SongQueueId") REFERENCES "SongQueue" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE TABLE "Guilds" (
        "GuildId" numeric(20,0) NOT NULL,
        "AllowSearchCommands" boolean NOT NULL,
        "AllowQuotes" boolean NOT NULL,
        "EnableQuoteChannel" boolean NOT NULL,
        "QuoteChannelId" numeric(20,0) NULL,
        "AllowAudio" boolean NOT NULL,
        "AudioPlayerId" uuid NULL,
        CONSTRAINT "PK_Guilds" PRIMARY KEY ("GuildId"),
        CONSTRAINT "FK_Guilds_AudioPlayer_AudioPlayerId" FOREIGN KEY ("AudioPlayerId") REFERENCES "AudioPlayer" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Guilds_Channels_QuoteChannelId" FOREIGN KEY ("QuoteChannelId") REFERENCES "Channels" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE TABLE "Quotes" (
        "QuoteId" uuid NOT NULL,
        "GuildId" numeric(20,0) NOT NULL,
        "Message" character varying(500) NOT NULL,
        "TimeAdded" timestamp with time zone NOT NULL,
        "BlamedUserId" numeric(20,0) NULL,
        CONSTRAINT "PK_Quotes" PRIMARY KEY ("QuoteId"),
        CONSTRAINT "FK_Quotes_Guilds_GuildId" FOREIGN KEY ("GuildId") REFERENCES "Guilds" ("GuildId") ON DELETE CASCADE,
        CONSTRAINT "FK_Quotes_Users_BlamedUserId" FOREIGN KEY ("BlamedUserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE INDEX "IX_AudioPlayer_SongQueueId" ON "AudioPlayer" ("SongQueueId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE INDEX "IX_AudioPlayer_VoiceChannelId" ON "AudioPlayer" ("VoiceChannelId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE INDEX "IX_Guilds_AudioPlayerId" ON "Guilds" ("AudioPlayerId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE INDEX "IX_Guilds_QuoteChannelId" ON "Guilds" ("QuoteChannelId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE INDEX "IX_Quotes_BlamedUserId" ON "Quotes" ("BlamedUserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE INDEX "IX_Quotes_GuildId" ON "Quotes" ("GuildId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    CREATE INDEX "IX_Song_SongQueueId" ON "Song" ("SongQueueId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210802172558_init') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20210802172558_init', '5.0.8');
    END IF;
END $$;
COMMIT;

