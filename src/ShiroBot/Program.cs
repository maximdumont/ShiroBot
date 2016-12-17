using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using NLog;
using NLog.Config;

namespace ShiroBot
{
    public class Program
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            // Configure logger
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "Config", "NLog.xml"));

            // Configure console
            Console.Title = "ShiroBot";
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Exit();
                eventArgs.Cancel = true;
            };

            // Load configuration
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "configuration.json");
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));

                Logger.Warn("A configuration file has been created for you, please modify it.");
                Logger.Warn($"Path: {configPath}");

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }

                return;
            }

            var configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configPath));
            var application = new Application(configuration);
            // Run program
            application.Run().GetAwaiter().GetResult();
            // Wait until close event
            QuitEvent.WaitOne();
            // Shutdown program
            application.Stop().GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Use this for emergency exits.
        /// </summary>
        public static void Exit()
        {
            QuitEvent.Set();
        }
    }
}
