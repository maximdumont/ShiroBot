using Newtonsoft.Json;

namespace ShiroBot
{
    public class Configuration
    {

        [JsonProperty("bot_token", Required = Required.AllowNull)]
        public string BotToken { get; set; }

    }
}
