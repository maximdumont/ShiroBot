using System;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using Discord;
using Discord.WebSocket;

namespace ShiroBot
{
    public class ShiroBot
    {
        private Logger _log;

        public static ShardedDiscordClient Client { get; private set; }

        static ShiroBot()
        {
            SetupLogger();
            Credentials = new BotCredentials();

            using (var uow = DbHandler.UnitOfWork())
            {
                AllGuildConfigs = uow.GuildConfigs.GetAll();
            }
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

            //connect
            await Client.LoginAsync(TokenType.Bot, Credentials.Token).ConfigureAwait(false);
            await Client.ConnectAsync().ConfigureAwait(false);
            await Client.DownloadAllUsersAsync().ConfigureAwait(false);

            _log.Info("Connected");
        }

        public async Task RunAndBlockAsync(params string[] args)
        {
            await RunAsync(args).ConfigureAwait(false);
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
