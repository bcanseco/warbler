using System;

namespace Warbler.Models
{
    [Obsolete("Currently unused")]
    public class Relationship
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string BlockedUserId { get; set; }
        public User BlockedUser { get; set; }
    }
}
