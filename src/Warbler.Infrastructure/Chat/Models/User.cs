using System.Collections.Generic;

namespace Warbler.Infrastructure.Chat.Models
{
    public class User
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }
        public string Name { get; set; }

        public ICollection<Membership> Memberships { get; set; }
    }
}
