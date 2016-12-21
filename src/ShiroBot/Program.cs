using System;
using System.Threading;

namespace ShiroBot
{
    public class Program
    {
        // Internal reference to bot name and version
        public const string botName = "ShiroBot";
        public const string botVersion = "0.0.1a";

        // Needed for thread control
        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        // Main application
        public static void Main(string[] args)
        {
            // Configure console (possibly remove this when we go to production?)
            Console.Title = botName + " - " + botVersion;
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Exit();
                eventArgs.Cancel = true;
            };

            // Instantiate a new ShiroBot
            var ShiroBot = new ShiroBot(new Configuration());

            // Run bot
            ShiroBot.Run().GetAwaiter().GetResult();

            // Wait until close event
            mre.WaitOne();

            // Shutdown bot
            ShiroBot.Stop().GetAwaiter().GetResult();

        }

        // Emergency stop
        public static void Exit()
        {
            mre.Set();
        }
    }
}
