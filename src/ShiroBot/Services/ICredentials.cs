using Discord;

namespace ShiroBot.Services
{
    public interface ICredentials
    {
        ulong ClientId { get; }
        ulong BotId { get; }

        string Token { get; }

        ulong[] OwnerIds { get; }

        bool IsOwner(IUser u);
    }
}
