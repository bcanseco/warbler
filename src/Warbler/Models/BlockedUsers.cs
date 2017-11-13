using System.Diagnostics;
using Newtonsoft.Json;

namespace Warbler.Models
{
    public class BlockedUsers
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int BlockedUserId { get; set; }
        [JsonIgnore]
        public User BlockedUser { get; set; }
    }
}
