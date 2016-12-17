using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System;
using System.Linq;

namespace ShiroBot.Commands
{
    public class InfoModule : ModuleBase
    {
        public async Task<IUserMessage> EmbedReplyAsync(EmbedBuilder embed, string msg = "")
            => await ReplyAsync("", embed: embed);

        // Pls don't modify, this will work after updating Discord.NET build after the build of (Friday, December 16, 2016 (12/16/2016))
        [Command("say"), Summary("Echos a message.")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        //Testing stats command
        [Command("stats"), Summary("Grabs Bot statistics.")]
        public async Task Stats()
        {
            var currUser = Application.currUser;
            var embed = new EmbedBuilder()
                .WithAuthor(x => x.WithName(currUser.Username).WithIconUrl(currUser.AvatarUrl).WithUrl("https://shirobot.xyz/"))
                .WithThumbnailUrl("http://i.imgur.com/T9BwNLI.png")
                .AddField(x => x.WithName("Guilds").WithValue(Application.currClient.Guilds.Count.ToString()).WithIsInline(true))
                .AddField(x => x.WithName("Commands").WithValue(Application.currCommandService.Commands.Count().ToString()).WithIsInline(true))
                .AddField(x => x.WithName("Statistics").WithValue($"Text Channels: {Application.TextChannels.ToString()}\nVoice Channels: {Application.VoiceChannels.ToString()}").WithIsInline(true))
                .AddField(x => x.WithName("Invite").WithValue($"<https://discordapp.com/oauth2/authorize?client_id={currUser.Id}&scope=bot>").WithIsInline(true))
                .WithColor(new Color(0, 255, 0))
                .WithTimestamp(DateTimeOffset.UtcNow);
            await EmbedReplyAsync(embed);
        }
    }
}
