using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace ShiroBot.Configuration
{
    public class ShiroBotConfiguration
    {
        // Private variable for logging in this class
        private static Logger s_log = LogManager.GetCurrentClassLogger();

        [JsonIgnore]
        public static readonly string s_ApplicationDirectory = AppContext.BaseDirectory;

        // Configuration items
        public HashSet<ulong> AdminIds { get; set; } = new HashSet<ulong>();
        public DiscordClientConfig DiscordClientConfig { get; set; } = new DiscordClientConfig();

        // Try to load configuration from a file
        public static ShiroBotConfiguration Load(string file = "ShiroBot.json")
        {
            string loc = Path.Combine(s_ApplicationDirectory, "Configuration", file);

            // Try to load the configuration and capture
            try
            {
                return JsonConvert.DeserializeObject<ShiroBotConfiguration>(File.ReadAllText(loc));
            }
            catch (Exception ex)
            {
                s_log.Fatal($"{ShiroBot.s_BotName} was unable to load configuration from {loc}. Exception thrown was:" + ex.ToString());
                return new ShiroBotConfiguration();
            }
        }

        // Try to save configuration to a file
        public void Save(string file = "ShiroBot.json")
        {
            string loc = Path.Combine(s_ApplicationDirectory, "Configuration", file);
            string json = ToJson();
            File.WriteAllText(loc, json);
        }

        // Return current configuration object as json
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    // Container configuration for DiscordClient
    public class DiscordClientConfig
    {
        public int audio_mode { get; set; }
        public int message_cache_size { get; set; }
        public bool download_users_on_guild_available { get; set; }
        public int log_level { get; set; }
        public int connection_timeout { get; set; }
        public string connection_token { get; set; }
    }

}