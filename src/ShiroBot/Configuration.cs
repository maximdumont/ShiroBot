using System;
using System.IO;
using Newtonsoft.Json;

namespace ShiroBot
{
    public class Configuration
    {
        // Hard code values for configuration and log directory names
        public const string ConfigurationDirectoryName = "Configuration";
        public const string LogDirectoryName = "Logs";

        // Variable used to show when a configuration load has failed due to missing file or bad configuration
        [JsonProperty("bad_configuration", Required = Required.Default)]
        public bool HasBadConfiguration { get; set; }

        // Properties to de/serialize to and from JSON
        [JsonProperty("bot_token", Required = Required.DisallowNull)]
        public string BotToken { get; set; }

        [JsonProperty("connection_timeout", Required = Required.AllowNull)]
        public int ConnectionTimeout { get; set; }

        [JsonProperty("log_level", Required = Required.DisallowNull)]
        public int LogLevel { get; set; } //  0 - 5

        [JsonProperty("environment", Required = Required.AllowNull)]
        public string Environment { get; set; } // production, development

        [JsonProperty("bot_url", Required = Required.AllowNull)]
        public string BotUrl { get; set; } // url where the bot web service will be running from

        // Load configuration from configuration.json and if it doesn't exist create a dummy one
        public Configuration LoadConfiguration()
        {
            var configurationFilePath = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationDirectoryName, "ShiroBot.json");

            // Make sure the configuration directory exists
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), ConfigurationDirectoryName));
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Creating configuration directory failed, exception thrown was: {0}", e.ToString());
                this.HasBadConfiguration = true;
                return this;
            }

            // Configuration file does not exist, create a blank one
            if (!File.Exists(configurationFilePath))
            {
                File.WriteAllText(configurationFilePath, JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));
                Console.WriteLine("No configuration file exists for " + Program.BotName + ".");
                Console.WriteLine("A blank configuration file has been created for you, please modify it.");
                Console.WriteLine($"Path: {configurationFilePath}");

                this.HasBadConfiguration = true;
                return this;
            }

            // Read bot configuration into memory and pass it to shirobot instantiation
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configurationFilePath));
        }
    }
}
