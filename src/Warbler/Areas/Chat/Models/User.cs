using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Warbler.Areas.Chat.Models
{
    [DebuggerDisplay("User #{Id}: {Name, nq}")]
    public class User
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }
        public string Name { get; set; }

        public ICollection<Membership> Memberships { get; set; }

        private List<Channel> _channels;

        public List<Channel> Channels
        {
            get { return _channels ?? (_channels = Memberships?.Select(m => m.Channel).ToList()); }
        }
    }
}
