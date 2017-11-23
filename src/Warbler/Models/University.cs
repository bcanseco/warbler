using System;
using System.Diagnostics;

namespace Warbler.Models
{
    [DebuggerDisplay("Uni #{Id}: {Name, nq}")]
    public class University : IEquatable<University>
    {
        public int Id { get; set; }
        public string PlaceId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public float Lat { get; set;  }
        public float Lng { get; set; }
        
        public Server Server { get; set; }
        
        public User ClaimedBy { get; set; }
        public string ClaimedById { get; set; }

        public bool Equals(University other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((University)obj);
        }

        public override int GetHashCode() => Id.GetHashCode() * 9;
        public static bool operator ==(University left, University right) => Equals(left, right);
        public static bool operator !=(University left, University right) => !Equals(left, right);
    }
}
