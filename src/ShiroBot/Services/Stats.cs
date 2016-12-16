using Discord;
using Discord.WebSocket;
using ShiroBot;
using ShiroBot.Extensions;
using ShiroBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ShiroBot.Services.Impl
{
    public class Stats : IStats
    {
        private ShardedDiscordClient client;
        private DateTime started;

        public const string BotVersion = "0.1-alpha";

        public string Author => "fkndean#7748";
        public string Library => "Discord.Net-beta2";
        public int MessageCounter { get; private set; } = 0;
        public int CommandsRan { get; private set; } = 0;
        public string Heap => Math.Round((double)GC.GetTotalMemory(false) / 1.MiB(), 2).ToString();
        public double MessagesPerSecond => MessageCounter / (double)GetUptime().TotalSeconds;
        public int TextChannels => client.GetGuilds().SelectMany(g => g.GetChannelsAsync().Result.Where(c => c is ITextChannel)).Count();
        public int VoiceChannels => client.GetGuilds().SelectMany(g => g.GetChannelsAsync().Result.Where(c => c is IVoiceChannel)).Count();
        public string OwnerIds => string.Join(", ", ShiroBot.Credentials.OwnerIds);



        Timer carbonitexTimer { get; }

        public Stats(ShardedDiscordClient client, CommandHandler cmdHandler)
        {

            this.client = client;

            Reset();
            this.client.MessageReceived += _ => Task.FromResult(MessageCounter++);
            cmdHandler.CommandExecuted += (_, e) => CommandsRan++;

            this.client.Disconnected += _ => Reset();

            this.carbonitexTimer = new Timer(async (state) =>
            {
                if (string.IsNullOrWhiteSpace(ShiroBot.Credentials.CarbonKey))
                    return;
                try
                {
                    using (var http = new HttpClient())
                    {
                        using (var content = new FormUrlEncodedContent(
                            new Dictionary<string, string> {
                                { "servercount", this.client.GetGuilds().Count.ToString() },
                                { "key", ShiroBot.Credentials.CarbonKey }}))
                        {
                            content.Headers.Clear();
                            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                            var res = await http.PostAsync("https://www.carbonitex.net/discord/data/botdata.php", content).ConfigureAwait(false);
                        }
                    };
                }
                catch { }
            }, null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        }
        public async Task<string> Print()
        {
            var curUser = await client.GetCurrentUserAsync();
            return $@"
Author: [{Author}] | Library: [{Library}]
Bot Version: [{BotVersion}]
Bot ID: {curUser.Id}
Owner ID(s): {OwnerIds}
Uptime: {GetUptimeString()}
Servers: {client.GetGuilds().Count} | TextChannels: {TextChannels} | VoiceChannels: {VoiceChannels}
Commands Ran this session: {CommandsRan}
Messages: {MessageCounter} [{MessagesPerSecond:F2}/sec] Heap: [{Heap} MB]";
        }

        public Task Reset()
        {
            MessageCounter = 0;
            started = DateTime.Now;
            return Task.CompletedTask;
        }

        public TimeSpan GetUptime() =>
            DateTime.Now - started;

        public string GetUptimeString(string separator = ", ")
        {
            var time = GetUptime();
            return $"{time.Days} days{separator}{time.Hours} hours{separator}{time.Minutes} minutes";
        }
    }
}