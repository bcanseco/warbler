using System;

namespace Warbler.Infrastructure.Chat.Models
{
    public class Channel : WarblerEntity
    {
        public int ServerId { get; set; }
        public DateTime LastUsed { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelState State { get; set; }
        public ChannelType Type { get; set; }

        public Channel(
            int id,
            int serverId,
            DateTime lastUsed,
            string name,
            string desc,
            int state,
            int type) : base(id)
        {
            ServerId = serverId;
            LastUsed = lastUsed;
            Name = name;
            Description = desc;
            State = (ChannelState) state;
            Type = (ChannelType) type;
        }
    }
}