using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using ShiroBot.Configuration;

namespace ShiroBot
{
    public class Program
    {
        // ShiroBot instance
        public static ShiroBot s_ShiroBot { get; set; }

        // Private variable for logging in this class
        private static Logger s_log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            // Setup console title
            Console.Title = $"{ShiroBot.s_BotName} - {ShiroBot.s_BotVersion}";

            // Starter Functions
            SetupLogging();
            InitializeConfiguration();

            // Instantiate and start a new instance of ShiroBot
            s_ShiroBot = new ShiroBot();
            StartShiroBotAsync();

            // Loop infinitely over console line text, if it's not exit, keep running and parsing commands
            string consoleLine = string.Empty;
            while (consoleLine != "exit")
            {
                // Read new console line in, and split input by spaces
                consoleLine = Console.ReadLine();
                string[] argv = consoleLine.Split(new char[] { ' ' }, 1);

                // Detect the following console commands
                switch (argv[0])
                {
                    case "start":
                        StartShiroBotAsync();
                        break;
                    case "stop":
                    case "exit":
                        StopShiroBotAsync();
                        break;
                    case "restart":
                        RestartShiroBotAsync();
                        break;
                    case "reload":
                        ReloadShiroBotAsync();
                        break;
                    default:
                        break;
                }
            }
        }

        // Start ShiroBot asynchronously
        public static async void StartShiroBotAsync()
        {
            await s_ShiroBot.StartAsync();
        }

        // Stop ShiroBot asynchronously
        public static async void StopShiroBotAsync()
        {
            await s_ShiroBot.StopAsync();
        }

        // Restart ShiroBot asynchronously
        public static async void RestartShiroBotAsync()
        {
            await s_ShiroBot.RestartAsync();
        }

        // Reload ShiroBot asynchronously
        public static void ReloadShiroBotAsync()
        {
            // TODO
            //await s_ShiroBot.ReloadAsync();
        }

        // A helper function to setup a new logconfiguration to use with the logmanager.
        public static void SetupLogging()
        {
            // Creating configuration
            var nLogConfiguration = new LoggingConfiguration();

            // Creating targets
            var consoleTarget = new ColoredConsoleTarget();
            nLogConfiguration.AddTarget("console", consoleTarget);
            var fileTarget = new FileTarget();
            nLogConfiguration.AddTarget("file", fileTarget);

            // Configure target properties
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} | ${message}";
            fileTarget.FileName = $"Logs/{ShiroBot.s_BotName}.log";
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} | ${message}";

            // Define logging rules
            nLogConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
            nLogConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            // Activate logging configuration§
            LogManager.Configuration = nLogConfiguration;
        }

        // Ensure bot configuration is there and can be loaded
        public static void InitializeConfiguration()
        {
            // Create configuration directory if it doesn't exist
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "Configuration")))
            {
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Configuration"));
            }

            string loc = Path.Combine(AppContext.BaseDirectory, "Configuration/ShiroBot.json");

            // If configuration file does't exist, create it and exit
            if (!File.Exists(loc))
            {
                var shiroBotConfiguration = new ShiroBotConfiguration();
                shiroBotConfiguration.Save();

                s_log.Fatal(
                    "A new configuration file has been created at ${loc}," +
                    "please ensure that all information has been updated and restart ${ShiroBot.s_BotName}.");
                Environment.Exit(0);
            }
            s_log.Info("Configuration Loaded");
        }
    }
}
