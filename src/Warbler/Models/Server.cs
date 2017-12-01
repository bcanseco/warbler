using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Warbler.Models
{
    [DebuggerDisplay("Server #{Id}")]
    public class Server : IEquatable<Server>
    {
        public int Id { get; set; }
        public bool IsAuthEnabled { get; set; }
        public int UniversityId { get; set; }
        
        [JsonIgnore]
        public University University { get; set; }
        public ICollection<Channel> Channels { get; set; }

        public bool Equals(Server other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Server) obj);
        }

        public override int GetHashCode() => Id.GetHashCode() * 9;
        public static bool operator ==(Server left, Server right) => Equals(left, right);
        public static bool operator !=(Server left, Server right) => !Equals(left, right);
    }
}
