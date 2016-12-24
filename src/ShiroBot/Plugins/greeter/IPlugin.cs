using Discord.WebSocket;

namespace ShiroBot.Plugins
{
    public interface IPlugin
    {
        DiscordSocketClient _discordClient { get; set; }
        void Init(object[] objectWrappers);
    }
}