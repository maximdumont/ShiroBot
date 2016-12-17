using System.Threading.Tasks;
using Discord.Commands;

namespace ShiroBot.Commands
{
    public class InfoModule : ModuleBase
    {
        // Pls don't modify, this will work after updating Discord.NET build after the build of (Friday, December 16, 2016 (12/16/2016))
        [Command("say"), Summary("Echos a message.")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }
    }
}
