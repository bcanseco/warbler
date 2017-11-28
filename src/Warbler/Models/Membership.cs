using System.Diagnostics;
using Newtonsoft.Json;

namespace Warbler.Models
{
    /// <summary>
    ///   Intermediate class linking the many-to-many relationship between users and channels.
    /// </summary>
    /// <seealso href="https://github.com/aspnet/EntityFramework/issues/5965#issuecomment-230908550"/>
    [DebuggerDisplay("User #{UserId} -> Channel #{ChannelId}")]
    public class Membership
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int ChannelId { get; set; }
        [JsonIgnore]
        public Channel Channel { get; set; }

        public string SamlName { get; set; }
    }
}
