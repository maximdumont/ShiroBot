using Discord.WebSocket;

namespace ShiroBot
{
    public class EventService
    {
        public EventService(DiscordSocketClient discordClient, PluginService pluginService)
        {
            // Register all discord client events here
            //discordClient.MessageReceived = 


            /**
                   public event Func<SocketChannel, Task> ChannelCreated;
        public event Func<SocketChannel, Task> ChannelDestroyed;
        public event Func<SocketChannel, SocketChannel, Task> ChannelUpdated;
        public event Func<Task> Connected;
        public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;
        public event Func<Exception, Task> Disconnected;
        public event Func<SocketGuild, Task> GuildAvailable;
        public event Func<SocketGuild, Task> GuildMembersDownloaded;
        public event Func<SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;
        public event Func<SocketGuild, Task> GuildUnavailable;
        public event Func<SocketGuild, SocketGuild, Task> GuildUpdated;
        public event Func<SocketGuild, Task> JoinedGuild;
        public event Func<int, int, Task> LatencyUpdated;
        public event Func<SocketGuild, Task> LeftGuild;
        public event Func<ulong, Optional<SocketMessage>, Task> MessageDeleted;
        public event Func<SocketMessage, Task> MessageReceived;
        public event Func<Optional<SocketMessage>, SocketMessage, Task> MessageUpdated;
        public event Func<ulong, Optional<SocketUserMessage>, SocketReaction, Task> ReactionAdded;
        public event Func<ulong, Optional<SocketUserMessage>, SocketReaction, Task> ReactionRemoved;
        public event Func<ulong, Optional<SocketUserMessage>, Task> ReactionsCleared;
        public event Func<Task> Ready;
        public event Func<SocketGroupUser, Task> RecipientAdded;
        public event Func<SocketGroupUser, Task> RecipientRemoved;
        public event Func<SocketRole, Task> RoleCreated;
        public event Func<SocketRole, Task> RoleDeleted;
        public event Func<SocketRole, SocketRole, Task> RoleUpdated;
        public event Func<SocketUser, SocketGuild, Task> UserBanned;
        public event Func<SocketUser, ISocketMessageChannel, Task> UserIsTyping;
        public event Func<SocketGuildUser, Task> UserJoined;
        public event Func<SocketGuildUser, Task> UserLeft;
        public event Func<Optional<SocketGuild>, SocketUser, SocketPresence, SocketPresence, Task> UserPresenceUpdated;
        public event Func<SocketUser, SocketGuild, Task> UserUnbanned;
        public event Func<SocketUser, SocketUser, Task> UserUpdated;
        public event Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;
        **/

        }

    }
}