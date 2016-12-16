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

        public string Author => "fkndean#7748, keyphact#0468";
        public string Library => "Discord.Net-beta2";
        public string AvatarURL => "http://i.imgur.com/x5Z2Tbs.jpg";
        public int MessageCounter { get; private set; } = 0;
        public int CommandsRan { get; private set; } = 0;
        public string Heap => Math.Round((double)GC.GetTotalMemory(false) / 1.MiB(), 2).ToString();
        public double MessagesPerSecond => MessageCounter / (double)GetUptime().TotalSeconds;
        public int TextChannels => client.GetGuilds().SelectMany(g => g.GetChannelsAsync().Result.Where(c => c is ITextChannel)).Count();
        public int VoiceChannels => client.GetGuilds().SelectMany(g => g.GetChannelsAsync().Result.Where(c => c is IVoiceChannel)).Count();
        public string OwnerIds => string.Join(", ", ShiroBot.Credentials.OwnerIds);

        public Stats(ShardedDiscordClient client)
        {

            this.client = client;

            Reset();
            this.client.MessageReceived += _ => Task.FromResult(MessageCounter++);

            this.client.Disconnected += _ => Reset();
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