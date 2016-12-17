using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using NLog;

namespace ShiroBot
{
    /// <summary>
    ///     This class is responsible for dealing with discord events.
    /// </summary>
    internal class EventHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly DiscordSocketClient _discordClient;

        private readonly CommandService _commandService;

        private readonly DependencyMap _dependencyMap;

        public EventHandler(DiscordSocketClient discordClient, CommandService commandService, DependencyMap dependencyMap)
        {
            _discordClient = discordClient;
            _commandService = commandService;
            _dependencyMap = dependencyMap;
        }

//        public async Task DiscordClientOnLog(LogMessage logMessage)
//        {
//            switch (logMessage.Severity)
//            {
//                case LogSeverity.Critical:
//                    Logger.Fatal(logMessage.Exception, logMessage.Message);
//                    break;
//                case LogSeverity.Error:
//                    Logger.Error(logMessage.Exception, logMessage.Message);
//                    break;
//                case LogSeverity.Warning:
//                    Logger.Warn(logMessage.Exception, logMessage.Message);
//                    break;
//                case LogSeverity.Info:
//                case LogSeverity.Verbose:
//                    Logger.Info(logMessage.Exception, logMessage.Message);
//                    break;
//                case LogSeverity.Debug:
//                    Logger.Debug(logMessage.Exception, logMessage.Message);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }

        public async Task DiscordClientOnMessageReceived(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos))) return;

            // Create a Command Context
            var context = new CommandContext(_discordClient, message);

            // Execute the command. (result does not indicate a return value, rather an object stating if the command executed succesfully)
            var result = await _commandService.ExecuteAsync(context, argPos, _dependencyMap);

            if (!result.IsSuccess)
            {
                var errorMessage = await message.Channel.SendMessageAsync($"{socketMessage.Author.Mention}: {result.ErrorReason}");

                await Task.Delay(2500).ContinueWith(async task =>
                {
                    await errorMessage.DeleteAsync();
                });
            }
        }
    }
}
