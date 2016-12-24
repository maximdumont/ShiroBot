using Discord.WebSocket;

namespace ShiroBot.Plugins
{
    public interface IPlugin
    {
        DiscordSocketClient DiscordClient { get; set; }
        void Init(object[] objectWrappers);
    }
}