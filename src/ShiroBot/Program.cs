using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ShiroBot
{
    public class Program
    {
        // Internal reference: bot information
        public const string botName = "ShiroBot";
        public const string botVersion = "0.0.1b";
        public const string botSupportUrl = "https://github.com/keyphact/ShiroBot/issues";

        // General ShiroBot configuration
        private static IConfigurationRoot _configuration;

        // Needed for thread control
        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        // Main application
        public static void Main(string[] args)
        {
            // Configure console
            // On ctrl-c unblock threads via (mre.Set) and continue to shutdown bot
            Console.Title = botName + " - " + botVersion;
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                mre.Set();
                eventArgs.Cancel = true;
            };

            // Load and build configuration into static variable to pass to ShiroBot
            try
            {
                _configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("Configuration/ShiroBot.json", optional: false, reloadOnChange: true)
                                    .Build();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Exception raised while trying to load bot configuration file. {0}", ex.ToString());
                System.Environment.Exit(-1);
            }

            // Instantiate a new ShiroBot
            var ShiroBot = new ShiroBot(_configuration);

            // Run ShiroBot
            Task.Run(async () =>
            {
                await ShiroBot.RunAsync();
            });

            // Block all threads here
            mre.WaitOne();

            // Shutdown ShiroBot, once mre has been set
            Task.Run(async () =>
            {
                await ShiroBot.StopAsync();
            });
        }
    }
}
