﻿using Discord;
using Discord.WebSocket;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiroBot
{
    public class ShardedDiscordClient
    {
        private DiscordSocketConfig discordSocketConfig;
        private Logger _log { get; }

        public event Func<IGuildUser, Task> UserJoined = delegate { return Task.CompletedTask; };
        public event Func<IMessage, Task> MessageReceived = delegate { return Task.CompletedTask; };
        public event Func<IGuildUser, Task> UserLeft = delegate { return Task.CompletedTask; };
        public event Func<SocketUser, SocketUser, Task> UserUpdated = delegate { return Task.CompletedTask; };
        public event Func<Optional<IMessage>, IMessage, Task> MessageUpdated = delegate { return Task.CompletedTask; };
        public event Func<ulong, Optional<IMessage>, Task> MessageDeleted = delegate { return Task.CompletedTask; };
        public event Func<IUser, IGuild, Task> UserBanned = delegate { return Task.CompletedTask; };
        public event Func<IUser, IGuild, Task> UserUnbanned = delegate { return Task.CompletedTask; };
        public event Func<Optional<SocketGuild>, SocketUser, SocketPresence, SocketPresence, Task> UserPresenceUpdated = delegate { return Task.CompletedTask; };
        public event Func<IUser, IVoiceState, IVoiceState, Task> UserVoiceStateUpdated = delegate { return Task.CompletedTask; };
        public event Func<IChannel, Task> ChannelCreated = delegate { return Task.CompletedTask; };
        public event Func<IChannel, Task> ChannelDestroyed = delegate { return Task.CompletedTask; };
        public event Func<IChannel, IChannel, Task> ChannelUpdated = delegate { return Task.CompletedTask; };
        public event Func<Exception, Task> Disconnected = delegate { return Task.CompletedTask; };

        private IReadOnlyList<DiscordSocketClient> Clients { get; }

        public ShardedDiscordClient(DiscordSocketConfig discordSocketConfig)
        {
            _log = LogManager.GetCurrentClassLogger();
            this.discordSocketConfig = discordSocketConfig;

            var clientList = new List<DiscordSocketClient>();
            for (int i = 0; i < discordSocketConfig.TotalShards; i++)
            {
                discordSocketConfig.ShardId = i;
                var client = new DiscordSocketClient(discordSocketConfig);
                clientList.Add(client);
                client.UserJoined += async arg1 => await UserJoined(arg1);
                client.MessageReceived += async arg1 => await MessageReceived(arg1);
                client.UserLeft += async arg1 => await UserLeft(arg1);
                client.UserUpdated += async (arg1, gu2) => await UserUpdated(arg1, gu2);
                client.MessageUpdated += async (arg1, m2) => await MessageUpdated((SocketMessage)arg1, m2);
                client.MessageDeleted += async (arg1, arg2) => await MessageDeleted(arg1, (SocketMessage)arg2);
                client.UserBanned += async (arg1, arg2) => await UserBanned(arg1, arg2);
                client.UserPresenceUpdated += async (arg1, arg2, arg3, arg4) => await UserPresenceUpdated(arg1, arg2, arg3, arg4);
                client.UserVoiceStateUpdated += async (arg1, arg2, arg3) => await UserVoiceStateUpdated(arg1, arg2, arg3);
                client.ChannelCreated += async arg => await ChannelCreated(arg);
                client.ChannelDestroyed += async arg => await ChannelDestroyed(arg);
                client.ChannelUpdated += async (arg1, arg2) => await ChannelUpdated(arg1, arg2);

                _log.Info($"Shard #{i} initialized.");
            }

            Clients = clientList.AsReadOnly();
        }

        public ISelfUser GetCurrentUser() =>
            Clients[0].CurrentUser;

        public Task<ISelfUser> GetCurrentUserAsync() =>
            Task.FromResult<ISelfUser>(Clients[0].CurrentUser);

        public Task<ISelfUser[]> GetAllCurrentUsersAsync() =>
            Task.WhenAll(Clients.Select(c => Task.FromResult<ISelfUser>(c.CurrentUser)));

        public IReadOnlyCollection<IGuild> GetGuilds() =>
            Clients.SelectMany(c => c.Guilds).ToArray();

        public IGuild GetGuild(ulong id) =>
            Clients.Select(c => c.GetGuild(id)).FirstOrDefault(g => g != null);

        public Task<IDMChannel> GetDMChannelAsync(ulong channelId) =>
            Clients[0].GetDMChannelAsync(channelId);

        internal Task LoginAsync(TokenType tokenType, string token) =>
            Task.WhenAll(Clients.Select(async c => { await c.LoginAsync(tokenType, token).ConfigureAwait(false); _log.Info($"Shard #{c.ShardId} logged in."); }));

        internal async Task ConnectAsync()
        {
            foreach (var c in Clients)
            {
                try
                {
                    await c.ConnectAsync().ConfigureAwait(false);
                    _log.Info($"Shard #{c.ShardId} connected.");
                }
                catch
                {
                    _log.Error($"Shard #{c.ShardId} FAILED CONNECTING.");
                    try { await c.ConnectAsync().ConfigureAwait(false); }
                    catch (Exception ex2)
                    {
                        _log.Error($"Shard #{c.ShardId} FAILED CONNECTING TWICE.");
                        _log.Error(ex2);
                    }
                }
            }
        }

        internal Task DownloadAllUsersAsync() =>
            Task.WhenAll(Clients.Select(async c => { await c.DownloadAllUsersAsync().ConfigureAwait(false); _log.Info($"Shard #{c.ShardId} downloaded {c.Guilds.Sum(g => g.Users.Count)} users."); }));
    }
}