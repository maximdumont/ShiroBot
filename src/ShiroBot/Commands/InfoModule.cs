using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using System;
using System.Linq;
using Discord.WebSocket;

namespace ShiroBot.Commands
{
    public class InfoModule : ModuleBase
    {
        private readonly CommandService _commands;

        private readonly DiscordSocketClient _discordClient;

        public InfoModule(CommandService commands, IDependencyMap map)
        {
            _commands = commands;
            _discordClient = map.Get<DiscordSocketClient>();
        }

        [Command("say"), Summary("Echos a message.")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        //Testing stats command
        [Command("stats"), Summary("Grabs bot statistics.")]
        public async Task Stats()
        {
            var currUser = _discordClient.CurrentUser;

            var guilds = _discordClient.Guilds.Count;
            var commands = _commands.Commands.Count();
            var textChannels = _discordClient.Guilds.SelectMany(x => x.GetTextChannelsAsync().Result).Count();
            var voiceChannels = _discordClient.Guilds.SelectMany(x => x.GetVoiceChannelsAsync().Result).Count();

            var embed = new EmbedBuilder()
                .WithAuthor(x => x.WithName(currUser.Username).WithIconUrl(currUser.AvatarUrl).WithUrl("https://shirobot.xyz/"))
                .WithThumbnailUrl("http://i.imgur.com/T9BwNLI.png")
                .AddField(x => x.WithName("Guilds").WithValue(guilds.ToString()).WithIsInline(true))
                .AddField(x => x.WithName("Commands").WithValue(commands.ToString()).WithIsInline(true))
                .AddField(x => x.WithName("Statistics").WithValue($"Text Channels: {textChannels}\nVoice Channels: {voiceChannels}").WithIsInline(true))
                .AddField(x => x.WithName("Invite").WithValue($"<https://discordapp.com/oauth2/authorize?client_id={currUser.Id}&scope=bot>").WithIsInline(true))
                .WithColor(new Color(0, 255, 0))
                .WithTimestamp(DateTimeOffset.UtcNow);

            await ReplyAsync(string.Empty, false, embed);
        }
    }
}
