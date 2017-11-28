using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Warbler.Models.Enums;

namespace Warbler.Models
{
    [DebuggerDisplay("Channel #{Id}: {Name, nq} ({State, nq}, {Type, nq})")]
    public class Channel : IEquatable<Channel>
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelState State { get; set; }
        public ChannelType Type { get; set; }
        public DateTime LastUsed { get; set; }

        [JsonIgnore]
        public Server Server { get; set; }
        [JsonIgnore]
        public ICollection<Membership> Memberships { get; set; }
        public ICollection<Message> Messages { get; set; }

        [NotMapped]
        public IEnumerable<User> Users => Memberships?.Select(m => m.User).ToList();

        [NotMapped]
        public IDictionary<string, string> SamlNames => Memberships?
            .ToDictionary(m => m.User.UserName, m => m.SamlName);

        public bool Equals(Channel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Channel) obj);
        }

        public override int GetHashCode() => Id.GetHashCode() * 9;
        public static bool operator ==(Channel left, Channel right) => Equals(left, right);
        public static bool operator !=(Channel left, Channel right) => !Equals(left, right);
    }
}
