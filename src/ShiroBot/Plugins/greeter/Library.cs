using Discord.WebSocket;
using NLog;

namespace ShiroBot.Plugins
{
    public class Greeter : IPlugin
    {
        public DiscordSocketClient DiscordClient { get; set; }
        private static Logger Log { get; set; }

        public void Init(object[] objectWrappers)
        {
            // load discord client
            DiscordClient = (DiscordSocketClient)objectWrappers[0];
            Log = LogManager.GetCurrentClassLogger();

            Log.Info("plugin loaded and was init`d");
        }
    }
}
