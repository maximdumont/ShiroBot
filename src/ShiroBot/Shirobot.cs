using System.Threading.Tasks;
using Discord.Audio;
using Discord.WebSocket;
using NLog;
using ShiroBot.Configuration;
using ShiroBot.Plugins;

namespace ShiroBot
{
    public class ShiroBot
    {
        // Internal bot information
        public static string s_BotName = "ShiroBot";
        public static string s_BotVersion = "0.0.2";
        public static string s_BotSupportUrl = "https://github.com/keyphact/ShiroBot/issues";

        // Private variable for logging in this class
        private static Logger s_log = LogManager.GetCurrentClassLogger();

        // ShiroBot Plugin Manager and Loader (with 400% more sugoiness)
        private static PluginManager s_RootPluginManager;
        private static PluginLoader s_RootPluginLoader;

        // Booleans to store ShiroBot state
        public static bool IsStopping = false;
        public static bool Stopped = true;

        // For discord client
        private readonly DiscordSocketClient _discordClient;

        // Setup ShiroBot to run
        public ShiroBot()
        {
            // Instantiate a new discord client with configuration
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                ConnectionTimeout = ShiroBotConfiguration.Load().DiscordClientConfig.connection_timeout,
                AudioMode = (AudioMode)ShiroBotConfiguration.Load().DiscordClientConfig.audio_mode,
                MessageCacheSize = ShiroBotConfiguration.Load().DiscordClientConfig.message_cache_size,
                DownloadUsersOnGuildAvailable = ShiroBotConfiguration.Load().DiscordClientConfig.download_users_on_guild_available,
                LogLevel = (Discord.LogSeverity)ShiroBotConfiguration.Load().DiscordClientConfig.log_level,
                ShardId = 1, // Hard coding this as stand-alone ShiroBot does not need more than one shard
                TotalShards = 1, // Hard coding this as stand-alone ShiroBot does not need more than one shard
            });
        }

        // Run ShiroBot
        public async Task StartAsync()
        {

            // Reset state
            if (ShiroBot.Stopped == true && ShiroBot.IsStopping == false)
            {
                ShiroBot.Stopped = false;
            }
            else
            {
                s_log.Fatal($"There is a problem starting {ShiroBot.s_BotName}, the stop states are corrupt.");
                return;
            }

            s_log.Info($"{ShiroBot.s_BotName} v{ShiroBot.s_BotVersion} is starting up...");

            try
            {
                // Try to connect to the discord network
                s_log.Debug("Attempting to connect to the Discord network.");
                await _discordClient.LoginAsync(Discord.TokenType.Bot, ShiroBotConfiguration.Load().DiscordClientConfig.connection_token);
                await _discordClient.ConnectAsync();
            }
            catch (System.Exception ex)
            {
                // An exception was thrown, catch it and attempt to cleanly stop the bot, then exit
                s_log.Fatal(ex, $"An exception was thrown whilst trying to connect to the Discord network. Exception thrown was: {ex.ToString()}");
                Stop();
            }
            finally
            {
                // For some reason we're in a disconnected state despite no exception being thrown,
                if (_discordClient.ConnectionState == Discord.ConnectionState.Disconnected)
                {
                    s_log.Fatal("No exception was thrown however, a connection could not be made to the Discord network.");
                    s_log.Fatal("Please report this as a bug here: " + ShiroBot.s_BotSupportUrl);
                    Stop();
                }
            }

            // Before logging that we are connected, check the state first
            if (_discordClient.ConnectionState == Discord.ConnectionState.Connected)
            {
                s_log.Info("Succesfully connected to the Discord network.");
            }

            /**
            * Begin plugin management and handling 
            */

            // Start up a new root plugin manager
            s_RootPluginManager = new PluginManager(s_log);
            s_RootPluginLoader = new PluginLoader(_discordClient);


            /**
            * Pluginmanager needs to handle all hooks and events and call on plugins,
            * it also needs to know the state of all plugins 
            * Pluginmanager will need a static class of discord client in order to handle up/down stream of commands and info
            * 
            * PluginLoader works on watching and loading plugins into the manager 
            *
            * Iplugin needs to be the interface that all plugins implement
            * 
            */

            // loader can then go like s_RootPluginLoader.LoadPlugin(adasdasdsadad); which will automatically be avail to manager for stuff like
            // s_RootPluginManager.listLoadedPlugins(); 

            //s_RootPluginManager;

            // Ok, we're going to need the plugin loader to load plugins into the plugin manager
            // FFF, and need to create a new IPlugin class 

            // Create a new instance of pluginmanager and load some plugins
            // s_pluginService = new PluginService(s_configuration, _discordClient);
            // s_pluginService.LoadPlugin("greeter");
        }

        // Teardown/Stop tasks that are common to both asynch/synch stop methods
        private void _stopCommon()
        {
            if (!ShiroBot.IsStopping)
            {
                s_log.Info($"Attempting to stop {ShiroBot.s_BotName}.");
                ShiroBot.IsStopping = true;
            }
        }

        // Stop the bot synchronously
        public void Stop()
        {
            _stopCommon();
            if (_discordClient.ConnectionState != Discord.ConnectionState.Disconnected)
            {
                s_log.Debug("Attempting to disconnect from Discord.");
                _discordClient.DisconnectAsync();
            }
            ShiroBot.Stopped = true;
            ShiroBot.IsStopping = false;
        }

        // Stop the bot asynchronously
        public async Task StopAsync()
        {
            _stopCommon();
            if (_discordClient.ConnectionState != Discord.ConnectionState.Disconnected)
            {
                s_log.Debug("Attempting to disconnect from Discord.");
                await _discordClient.DisconnectAsync();
            }
            ShiroBot.Stopped = true;
            ShiroBot.IsStopping = false;
        }

        // Stop the bot asynchronously
        public async Task RestartAsync()
        {
            await this.StopAsync();
            await this.StartAsync();
        }

        // Reload the bot asynchronously
        public void ReloadAsync()
        {
            //TODO
        }

    }
}
