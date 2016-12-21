using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ShiroBot
{
    public class ShiroBot
    {
        // Private variable for logging throughout main ShiroBot class
        private static Logger _log;

        // For internal services
        private readonly WebService _webService;
        //private readonly CommandService _commandService;

        // For clients
        private readonly DiscordSocketClient _discordClient;

        // Bot Configuration
        private readonly Configuration _configuration;

        // Setup ShiroBot to run
        public ShiroBot(Configuration configuration)
        {
            // Load up bot configuration into local variable
            this._configuration = configuration.loadConfiguration();

            // If the configuration failed to load for whatever reason, exit.
            // The reason for exiting will have already been printed in loadConfiguration().
            if (_configuration.hasBadConfiguration)
            {
                System.Environment.Exit(-1);
            }

            // Setup logging (for ze world - wrryy)
            _setupLogging();

            // Instantiate a new discord client with configuration
            this._discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                // Audio mode, not documented (AS EXPECTED, THOSE JERKS) believed to be something to do with duplexing
                // Disabling for now as we don't use audio at all with ShiroBot yet.
                AudioMode = AudioMode.Disabled,
                MessageCacheSize = 100, // Caches 100 messages per channel, if we run into memory issues tweak it
                DownloadUsersOnGuildAvailable = true, // Self-explantory  
                LogLevel = (Discord.LogSeverity)_configuration.logLevel, // An explicit cast configuration file won't know about LogSeverity
                ShardId = 1,
                TotalShards = 1,
            });

            // Start up internal web service
            this._webService = new WebService();

            // Start up internal command service
            //this._commandService = new CommandService(_log);
        }

        // Run ShiroBot
        public async Task Run()
        {
            // Grab a logger
            _log = LogManager.GetCurrentClassLogger();
            _log.Info(Program.botName + " v" + Program.botVersion + " is starting up...");

            try
            {
                // Authenticate and connect to Discord
                await _discordClient.LoginAsync(TokenType.Bot, _configuration.botToken);
                await _discordClient.ConnectAsync();
                _log.Debug("Attempting to connect to Discord.");
            }
            catch (System.TimeoutException e)
            {
                // Timeout exception
                _log.Fatal("Timeout was reached trying to connet to Discord. Exception thrown was: {0}", e.ToString());
                System.Environment.Exit(-1);
            }

            // Start webservice but do not block on it.
            await Task.Run(() =>
                {
                    _webService.BuildandRun(); // testing without awaiting, this may break things
                });

            _log.Info("Succesfully connected to Discord.");

            // Discover all of the commands in this assembly and load them.
            //await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
            _log.Debug("Discovering all commands in current assembly.");

            // Show information
            _log.Info($"{"ClientID:",-10} {_discordClient.CurrentUser.Id}");
            _log.Info($"{"Username:",-10} {_discordClient.CurrentUser.Username}");
            _log.Info($"{"Guilds:",-10} {_discordClient.Guilds.Count}");
            //_log.Info($"{"Commands:",-10} {_commandService.Commands.Count()}");

            // Register events
            // _discordClient.Log += _eventHandler.DiscordClientOnLog;
            //_discordClient.MessageReceived += _eventHandler.DiscordClientOnMessageReceived;




        }

        // Stop the bot
        public async Task Stop()
        {
            await _discordClient.DisconnectAsync();
        }



        /**
        * Helper Functions (TODO: List them out)
        **/

        // A helper function to setup a new logmanager configuration for all of ShiroBot to use.
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
            catch (System.Exception e)
            {
                System.Console.WriteLine("The following exception was thrown when trying to setup logger for " + Program.botName + ": " + e.ToString());
                System.Environment.Exit(-1);
            }
        }

    }
}
