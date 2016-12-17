using System;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using Discord;
using Discord.WebSocket;
using Shirobot.Services;
using ShiroBot.Services.Impl;
using Discord.Commands;
using ShiroBot.Extensions;

namespace ShiroBot
{
    public class ShiroBot
    {
        private Logger _log;

        public static ShardedDiscordClient Client { get; private set; }
        public static Credentials Credentials { get; private set; }
        public static Stats Stats { get; private set; }
        public static CommandService CommandService { get; private set; }
        public static bool Ready { get; private set; } = false;

        static ShiroBot()
        {
            SetupLogger();
            Credentials = new Credentials();
        }


        public async Task RunAsync(params string[] args)
        {
            _log = LogManager.GetCurrentClassLogger();

            _log.Info("Starting ShiroBot v" + Stats.BotVersion);

            //create client
            Client = new ShardedDiscordClient(new DiscordSocketConfig
            {
                AudioMode = Discord.Audio.AudioMode.Outgoing,
                MessageCacheSize = 10,
                LogLevel = LogSeverity.Warning,
                TotalShards = Credentials.TotalShards,
                ConnectionTimeout = int.MaxValue
            });

            //initialize service
            Stats = new Stats(Client);

            //connect
            await Client.LoginAsync(TokenType.Bot, Credentials.Token).ConfigureAwait(false);
            await Client.ConnectAsync().ConfigureAwait(false);
            await Client.DownloadAllUsersAsync().ConfigureAwait(false);

            _log.Info("Connected");
            Ready = true;
        }

        public async Task RunAndBlockAsync(params string[] args)
        {
            await RunAsync(args).ConfigureAwait(false);
            //test send channel message with stats. //kp guildid to bot channel id.
            try
            {
                var curUser = await Client.GetCurrentUserAsync().ConfigureAwait(true);
                var stats = Stats;
                var embed = new EmbedBuilder()
                    .WithAuthor(x => x.WithName(curUser.Username).WithIconUrl(curUser.AvatarUrl))
                    .WithThumbnailUrl(stats.AvatarURL)
                    .WithColor(new Color(0, 255, 0))
                    .AddField(x => x.WithName("__Library__").WithValue(stats.Library).WithIsInline(false))
                    .AddField(x => x.WithName("__Bot Version__").WithValue(Stats.BotVersion).WithIsInline(false))
                    .AddField(x => x.WithName("__Bot ID__").WithValue(curUser.Id.ToString()).WithIsInline(false))
                    .AddField(x => x.WithName("__Owner ID(s)__").WithValue(stats.OwnerIds).WithIsInline(false))
                    .AddField(x => x.WithName("__Uptime__").WithValue(stats.GetUptimeString()).WithIsInline(false))
                    .AddField(x => x.WithName("__Statistics__").WithValue($"Servers: {Client.GetGuilds().Count}\nText Channels: {stats.TextChannels}\nVoice Channels: {stats.VoiceChannels}").WithIsInline(false))
                    .AddField(x => x.WithName("__Messages__").WithValue($"{stats.MessageCounter} [{stats.MessagesPerSecond:F2}/sec] Heap: [{stats.Heap} MB]").WithIsInline(false));
                var channel = await Client.GetGuild(156487862081028096).GetChannelAsync(259070878334189568) as ITextChannel;
                await channel.EmbedAsync(embed);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            await Task.Delay(-1).ConfigureAwait(false);
        }

        private static void SetupLogger()
        {
            try
            {
                var logConfig = new LoggingConfiguration();
                var consoleTarget = new ColoredConsoleTarget();

                consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} | ${message}";

                logConfig.AddTarget("Console", consoleTarget);

                logConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

                LogManager.Configuration = logConfig;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
