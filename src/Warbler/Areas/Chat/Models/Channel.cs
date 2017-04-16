using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Warbler.Areas.Chat.Models.Enums;

namespace Warbler.Areas.Chat.Models
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
        public ICollection<Message> Messages { get; set; }

        private List<User> _users;

        public List<User> Users
        {
            get { return _users ?? (_users = Memberships?.Select(m => m.User).ToList()); }
        }
    }
}
