using System.Collections;

namespace ShiroBot.Services.WebService.Models
{
    public class PermissionsBitwise
    {
        /// <summary>
        /// Allows kicking members
        /// </summary>
        public int KICK_MEMBERS { get; private set; }

        /// <summary>
        /// Allows banning members
        /// </summary>
        public int BAN_MEMBERS { get; private set; }

        /// <summary>
        /// Allows all permissions and bypasses channel permission overwrites
        /// </summary>
        public int ADMINISTRATOR { get; private set; }

        /// <summary>
        /// Allows management and editing of channels
        /// </summary>
        public int MANAGE_CHANNELS { get; private set; }

        /// <summary>
        /// Allows management and editing of the guild
        /// </summary>
        public int MANAGE_GUILD { get; private set; }

        /// <summary>
        /// Allows for the addition of reactions to messages
        /// </summary>
        public int ADD_REACTIONS { get; private set; }

        /// <summary>
        /// Allows reading messages in a channel. The channel will not appear for users without this permission
        /// </summary>
        public int READ_MESSAGES { get; private set; }

        /// <summary>
        /// Allows for sending messages in a channel.
        /// </summary>
        public int SEND_MESSAGES { get; private set; }

        /// <summary>
        /// Allows for sending of /tts messages
        /// </summary>
        public int SEND_TTS_MESSAGES { get; private set; }

        /// <summary>
        /// Allows for deletion of other users messages
        /// </summary>
        public int MANAGE_MESSAGES { get; private set; }

        /// <summary>
        /// Links sent by this user will be auto-embedded
        /// </summary>
        public int EMBED_LINKS { get; private set; }

        /// <summary>
        /// Allows for uploading images and files
        /// </summary>
        public int ATTACH_FILES { get; private set; }

        /// <summary>
        /// Allows for reading of message history
        /// </summary>
        public int READ_MESSAGE_HISTORY { get; private set; }

        /// <summary>
        /// Allows the usage of custom emojis from other servers
        /// </summary>
        public int MENTION_EVERYONE { get; private set; }

        /// <summary>
        /// Allows for joining of a voice channel
        /// </summary>
        public int USE_EXTERNAL_EMOJIS { get; private set; }

        /// <summary>
        /// Allows for speaking in a voice channel
        /// </summary>
        public int CONNECT { get; private set; }

        /// <summary>
        /// Allows kicking members
        /// </summary>
        public int SPEAK { get; private set; }

        /// <summary>
        /// Allows for muting members in a voice channel
        /// </summary>
        public int MUTE_MEMBERS { get; private set; }

        /// <summary>
        /// Allows for deafening of members in a voice channel
        /// </summary>
        public int DEAFEN_MEMBERS { get; private set; }

        /// <summary>
        /// Allows for moving of members between voice channels
        /// </summary>
        public int MOVE_MEMBERS { get; private set; }

        /// <summary>
        /// Allows for using voice-activity-detection in a voice channel
        /// </summary>
        public int USE_VAD { get; private set; }

        /// <summary>
        /// Allows for modification of own nickname
        /// </summary>
        public int CHANGE_NICKNAME { get; private set; }

        /// <summary>
        /// Allows for modification of other users nicknames
        /// </summary>
        public int MANAGE_NICKNAME { get; private set; }

        /// <summary>
        /// Allows management and editing of roles
        /// </summary>
        public int MANAGE_ROLES { get; private set; }

        /// <summary>
        /// Allows management and editing of webhooks
        /// </summary>
        public int MANAGE_WEBHOOKS { get; private set; }

        /// <summary>
        /// Allows management and editing of emojis
        /// </summary>
        public int MANAGE_EMOJIS { get; private set; }

        public IList getList { get; private set; }

        public PermissionsBitwise()
        {
            KICK_MEMBERS = 0x00000002;
            BAN_MEMBERS = 0x00000004;
            ADMINISTRATOR = 0x00000008;
            MANAGE_CHANNELS = 0x00000010;
            MANAGE_GUILD = 0x00000020;
            ADD_REACTIONS = 0x00000040;
            READ_MESSAGES = 0x00000400;
            SEND_MESSAGES = 0x00000800;
            SEND_TTS_MESSAGES = 0x00001000;
            MANAGE_MESSAGES = 0x00002000;
            EMBED_LINKS = 0x00004000;
            ATTACH_FILES = 0x00008000;
            READ_MESSAGE_HISTORY = 0x00010000;
            MENTION_EVERYONE = 0x00020000;
            USE_EXTERNAL_EMOJIS = 0x00040000;
            CONNECT = 0x00100000;
            SPEAK = 0x00200000;
            MUTE_MEMBERS = 0x00400000;
            DEAFEN_MEMBERS = 0x00800000;
            MOVE_MEMBERS = 0x01000000;
            USE_VAD = 0x02000000;
            CHANGE_NICKNAME = 0x04000000;
            MANAGE_NICKNAME = 0x08000000;
            MANAGE_ROLES = 0x10000000;
            MANAGE_WEBHOOKS = 0x20000000;
            MANAGE_EMOJIS = 0x40000000;
        }

        public int allPermissions()
        {
            return KICK_MEMBERS ^ BAN_MEMBERS ^ ADMINISTRATOR ^ MANAGE_CHANNELS ^ MANAGE_GUILD ^ ADD_REACTIONS ^ READ_MESSAGES ^ SEND_MESSAGES ^ SEND_TTS_MESSAGES ^ MANAGE_MESSAGES ^ EMBED_LINKS ^ 
                ATTACH_FILES ^ READ_MESSAGE_HISTORY ^ MENTION_EVERYONE ^ USE_EXTERNAL_EMOJIS ^ CONNECT ^ SPEAK ^ MUTE_MEMBERS ^ DEAFEN_MEMBERS ^ MOVE_MEMBERS ^ USE_VAD ^ CHANGE_NICKNAME ^ MANAGE_NICKNAME ^ MANAGE_ROLES ^ MANAGE_WEBHOOKS ^ MANAGE_EMOJIS;
        }
        public int allPermissions_NoAdmin()
        {
            return KICK_MEMBERS ^ BAN_MEMBERS ^ MANAGE_CHANNELS ^ MANAGE_GUILD ^ ADD_REACTIONS ^ READ_MESSAGES ^ SEND_MESSAGES ^ SEND_TTS_MESSAGES ^ MANAGE_MESSAGES ^ EMBED_LINKS ^
                ATTACH_FILES ^ READ_MESSAGE_HISTORY ^ MENTION_EVERYONE ^ USE_EXTERNAL_EMOJIS ^ CONNECT ^ SPEAK ^ MUTE_MEMBERS ^ DEAFEN_MEMBERS ^ MOVE_MEMBERS ^ USE_VAD ^ CHANGE_NICKNAME ^ MANAGE_NICKNAME ^ MANAGE_ROLES ^ MANAGE_WEBHOOKS ^ MANAGE_EMOJIS;
        }
    }
}
