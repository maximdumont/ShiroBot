using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord.Audio;
using Discord.WebSocket;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ShiroBot
{
    public class ShiroBot
    {
        // Private variable for logging in this class
        private static Logger s_log;

        // General ShiroBot configuration
        private static IConfigurationRoot s_configuration;

        // For ShiroBot internal services
        private static WebService s_webService;
        private static PluginService s_pluginService;

        // For discord client
        private readonly DiscordSocketClient _discordClient;

        // Setup ShiroBot to run
        public ShiroBot(IConfigurationRoot configuration)
        {
            // Copy configuration to internal static variable
            s_configuration = configuration;

            // Setup logging
            _setupLogging();

            // Instantiate a new discord client with configuration
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                ConnectionTimeout = s_configuration.GetValue<int>("discord_client:connection_timeout", 10000),
                AudioMode = (AudioMode)s_configuration.GetValue<int>("discord_client:audio_mode", 0),
                MessageCacheSize = s_configuration.GetValue<int>("discord_client:message_cache_size", 100),
                DownloadUsersOnGuildAvailable = s_configuration.GetValue<bool>("discord_client:download_users_on_guild_available", true),
                LogLevel = (Discord.LogSeverity)s_configuration.GetValue<int>("discord_client:log_level", 4),
                ShardId = 1, // Hard coding this as stand-alone ShiroBot does not need more than one shard
                TotalShards = 1, // Hard coding this as stand-alone ShiroBot does not need more than one shard
            });
        }

        // Run ShiroBot
        public async Task RunAsync()
        {
            // Grab a logger
            s_log = LogManager.GetCurrentClassLogger();
            s_log.Info(Program.BotName + " v" + Program.BotVersion + " is starting up...");

            try
            {
                // Try to connect to the discord network
                s_log.Debug("Attempting to connect to Discord.");
                await _discordClient.LoginAsync(Discord.TokenType.Bot, s_configuration.GetValue<string>("discord_client:connection_token"));
                await _discordClient.ConnectAsync();
            }
            catch (System.Exception ex)
            {
                // An exception was thrown, catch it and attempt to cleanly stop the bot, then exit
                s_log.Fatal("An exception was thrown whilst trying to connect to Discord. Exception thrown was: {0}", ex.ToString());
                await StopAsync();
                System.Environment.Exit(-1);
            }
            finally
            {
                if (_discordClient.ConnectionState == Discord.ConnectionState.Disconnected)
                {
                    s_log.Fatal("No exception was thrown however, a connection could not be made to the Discord network.");
                    s_log.Fatal("Please report this as a bug here: " + Program.BotSupportUrl);
                    System.Environment.Exit(-1);
                }
            }

            // Start webservice but do not block on it.
            //await _webService.BuildandRun(); // testing without awaiting, this may break things

            // Before logging that we are connected, check the state first
            if (_discordClient.ConnectionState == Discord.ConnectionState.Connected)
            {
                s_log.Info("Succesfully connected to the Discord network.");
            }

            // Create a new instance of pluginservice and load some plugins
            s_pluginService = new PluginService(s_configuration, _discordClient);
            s_pluginService.LoadPlugin("greeter");

            //_pluginService.loadAvailablePlugins(); -- implement a directory walk for *.dll files

            // PSUDEO CODE
            // var eventServiceHandler = new EventService(_discordClient);
            /**

            Something like.!--.!--.!--.!--

            eventService = new EventService();
            pluginServiceHost = new PluginService();

            foreach (pluginServiceHost.getResolvedPlugin()) 
                eventService.addListner(pluginServiceHost.getResolvedPlugin().pluginName)

            // Now need to create a stats service as well  
            // Place holder code for web and plugin service
            // Place holder to start up events and delegates code
            // Register events
            // _discordClient.Log += _eventHandler.DiscordClientOnLog;
            //_discordClient.MessageReceived += _eventHandler.DiscordClientOnMessageReceived;
            **/
        }

        // Stop the bot
        public async Task StopAsync()
        {
            s_log.Debug("Attempting to disconnect from Discord.");
            await _discordClient.DisconnectAsync();
            // await ShiroBot.PluginService.StopAsync();
            // await ShiroBot.WebService.StopAsync();
        }

        /**
        * A few private helper functions (TODO: List them out)
        **/

        // A helper function to setup a new logconfiguration for all of ShiroBot to use.
        private static void _setupLogging()
        {
            try
            {
                var logConfiguration = new LoggingConfiguration();
                var consoleTarget = new ColoredConsoleTarget();

                consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} | ${message}";
                logConfiguration.AddTarget("Console", consoleTarget);
                logConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
                LogManager.Configuration = logConfiguration;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("The following exception was thrown when trying to setup logger: " + ex.ToString());
                System.Environment.Exit(-1);
            }
        }
    }
}
