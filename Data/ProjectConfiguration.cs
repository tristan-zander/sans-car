using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Data
{
    public static class ProjectConfiguration
    {
        public static FullConfiguration ReadConfigFile(string configPath = "")
        {
            if (configPath.Length <= 0)
            {
                // Get project root.
                // Config path is expected to be in the current working directory.
                // TODO: Read this from an environment variable or command line switch.
                configPath = Path.GetFullPath("./config.json", Directory.GetCurrentDirectory());
            }

            var file = File.ReadAllText(configPath);
            var deserializedConfig =
                JsonSerializer.Deserialize<FullConfiguration>(file, new JsonSerializerOptions { IncludeFields = true });

            if (deserializedConfig == null)
            {
                throw new Exception(
                    "There was an error in your config file and it could not be properly deserialized.");
            }

            return deserializedConfig;
        }
    }

    public sealed class FullConfiguration
    {
        [JsonProperty("Bot")] public BotConfiguration Bot { get; set; }
        [JsonProperty("Lavalink")] public LavalinkConfiguration Lavalink { get; set; }
        [JsonProperty("Database")] public Database Database { get; set; }
    }

    public sealed class WebsiteConfiguration
    {
        [JsonInclude]
        [JsonProperty("StaticFiles")]
        public string StaticFilePath { get; set; }
    }

    public sealed class LavalinkConfiguration
    {
        [JsonInclude] public int Port { get; set; }
        [JsonInclude] public string Password { get; set; }
        [JsonInclude] public string Address { get; set; }
    }


    public sealed class BotConfiguration
    {
        [JsonInclude] public string[] Prefixes { get; set; }
        [JsonInclude] public string Token { get; set; }
    }

    public sealed class Database
    {
        public string ConnectionString { get; set; }
    }
}