using System.Collections.Generic;
using System.Diagnostics;

namespace Warbler.Areas.Chat.Models
{
    [DebuggerDisplay("User #{Id}: {Name, nq}")]
    public class User
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }
        public string Name { get; set; }

        public ICollection<Membership> Memberships { get; set; }
    }
}
