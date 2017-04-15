using System;
using System.Collections.Generic;
using System.Diagnostics;
using Warbler.Infrastructure.Chat.Models.Enums;

namespace Warbler.Infrastructure.Chat.Models
{
    [DebuggerDisplay("Channel #{Id}: {Name, nq} ({State, nq}, {Type, nq})")]
    public class Channel
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelState State { get; set; }
        public ChannelType Type { get; set; }
        public DateTime LastUsed { get; set; }

        public Server Server { get; set; }
        public ICollection<Membership> Memberships { get; set; }
    }
}
