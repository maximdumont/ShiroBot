using Discord.WebSocket;
using NLog;

namespace ShiroBot.Plugins
{
    public class Greeter : IPlugin
    {
        public DiscordSocketClient _discordClient { get; set; }
        static Logger _log { get; set; }

        public void Init(object[] objectWrappers)
        {
            // load discord client
            _discordClient = (DiscordSocketClient)objectWrappers[0];
            _log = LogManager.GetCurrentClassLogger();

            _log.Info("plugin loaded and was init`d");
        }
    }
}
