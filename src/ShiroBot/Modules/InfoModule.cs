// using System.Threading.Tasks;
// using Discord.Commands;
// using Discord;
// using System;
// using System.IO;
// using System.Linq;
// using Discord.WebSocket;
// using System.Net.Http;
// using Discord.API;

// namespace ShiroBot.Commands
// {
//     public class InfoModule : ModuleBase
//     {
//         private readonly CommandService _commands;

//         private readonly DiscordSocketClient _discordClient;

//         public InfoModule(CommandService commands, IDependencyMap map)
//         {
//             _commands = commands;
//             _discordClient = map.Get<DiscordSocketClient>();
//         }


//         [Command("say"), Summary("Echos a message.")]
//         [Alias("echo")]
//         public async Task Say([Summary("The (optional) user to get info for")] IUser user, [Remainder, Summary("The text to echo")] string echo)
//         {
//             try
//             {
//                 // ReplyAsync is a method on ModuleBase
//                 var userInfo = user ?? Context.Client.CurrentUser;
//                 await ReplyAsync($"{userInfo.Mention}: {echo}");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.ToString());
//                 await ReplyAsync("User not found.");
//             }
//         }

//         //Testing changing bot(self) avatar.
//         [Command("setavatar"), Summary("Sets bot avatar.")]
//         public async Task SetAvatar(string img)
//         {
//             using (var http = new HttpClient())
//             {
//                 using (var sr = await http.GetStreamAsync(img))
//                 {
//                     var memImg = new MemoryStream();
//                     await sr.CopyToAsync(memImg);
//                     memImg.Position = 0;
//                     await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(memImg));
//                 }
//             }

//             await ReplyAsync("Successfully changed avatar.");
//         }

//         //Testing changing bot(self) game
//         [Command("setgame"), Summary("Sets bot game.")]
//         public async Task SetGame(string game)
//         {
//             try
//             {
//                 DiscordSocketClient user = _discordClient;
//                 await user.SetGame(game, "https://www.twitch.tv/shirobot_xyz", StreamType.Twitch);

//                 await ReplyAsync("Successfully changed bot game status.");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine((ex.ToString()));
//             }
//         }

//         //Grab user avatar
//         [Command("getavatar"), Summary("Grabs user avatar url.")]
//         public async Task GetAvatar(IUser user)
//         {
//             var currUser = user ?? Context.Client.CurrentUser;
//             var embed = new EmbedBuilder()
//                 .WithAuthor(x => x.WithName(_discordClient.CurrentUser.Username).WithIconUrl(_discordClient.CurrentUser.AvatarUrl).WithUrl("https://shirobot.xyz/"))
//                 .WithThumbnailUrl(currUser.AvatarUrl)
//                 .WithColor(new Color(0, 255, 0))
//                 .WithTitle("🙌 **__Avatar Found__** 🙌")
//                 .AddField(x => x.WithName($"{currUser.Username}#`{currUser.Discriminator}` [{currUser.Id}]").WithValue($"<{currUser.AvatarUrl}>"));
//             await ReplyAsync(string.Empty, false, embed);
//         }

//         //Testing prune command
//         [Command("prune"), Summary("Prunes messages.")]
//         [Alias("delete")]
//         public async Task Messages(IUser user, int count = 5)
//         {
//             try
//             {
//                 var channel = (ITextChannel)Context.Message.Channel;
//                 var enumerable =
//                     channel.GetMessagesAsync(limit: count + 1)
//                         .ToEnumerable()
//                         .SelectMany(x => x.Where(y => y.Author.Id == user.Id));
//                 await channel.DeleteMessagesAsync(enumerable).ConfigureAwait(true);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.ToString());
//             }
//         }

//         //Testing stats command
//         [Command("stats"), Summary("Grabs bot statistics.")]
//         [Alias("statistics")]
//         public async Task Stats()
//         {
//             //Grab the channel where the command was run.
//             IMessageChannel channel = Context.Message.Channel;
//             //Convert to ITextChannel so I can convert it to an IGuild
//             var localchann = (ITextChannel)channel;
//             var localguild = (IGuild)localchann.Guild;
//             //I wasn't really quite too sure, I wanted to make sure that it's grabbing local guild information.

//             var currUser = _discordClient.CurrentUser;

//             var guilds = _discordClient.Guilds.Count;
//             var commands = _commands.Commands.Count();
//             var textChannels = _discordClient.Guilds.SelectMany(x => x.GetTextChannelsAsync().Result).Count();
//             var localTextChannels = localguild.GetTextChannelsAsync().GetAwaiter().GetResult().Count();
//             var voiceChannels = _discordClient.Guilds.SelectMany(x => x.GetVoiceChannelsAsync().Result).Count();
//             var voiceTextChannels = localguild.GetVoiceChannelsAsync().GetAwaiter().GetResult().Count();
//             var gatewayLatency = _discordClient.Latency;

//             var embed = new EmbedBuilder()
//                 .WithAuthor(x => x.WithName(currUser.Username).WithIconUrl(currUser.AvatarUrl).WithUrl("https://shirobot.xyz/"))
//                 .WithThumbnailUrl("http://i.imgur.com/T9BwNLI.png")
//                 //.WithDescription($"⚡ `Discord Gateway API` [**{gatewayLatency}**`ms`]\n💬 `Total Messages` {Application.MessageCounter}")
//                 .AddField(x => x.WithName($"🌐 Global").WithValue($"**__Guilds__**: {guilds.ToString()}\n**__Commands__**: {commands.ToString()}\n**__Text Channels__**: {textChannels}\n**__Voice Channels__**: {voiceChannels}").WithIsInline(false))
//                 .AddField(x => x.WithName($"🏠 Local").WithValue($"**__Guild__**: {localguild.ToString()}\n**__GuildID__**: {localguild.Id}\n**__Text Channels__**: {localTextChannels}\n**__Voice Channels__**: {voiceTextChannels}").WithIsInline(false))
//                 .WithFooter(x => x.WithText($"©️ Shiro Bot"))
//                 .WithColor(new Color(0, 255, 0))
//                 .WithTimestamp(DateTimeOffset.UtcNow);

//             await ReplyAsync(string.Empty, false, embed);
//         }
//     }
// }
