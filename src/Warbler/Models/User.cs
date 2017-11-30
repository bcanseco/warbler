using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;
using Warbler.Models.Enums;

namespace Warbler.Models
{
    [DebuggerDisplay("User #{Id}: {UserName, nq}")]
    [DataContract]
    public class User : IdentityUser, IEquatable<User>
    {
        /// <remarks>
        ///   Overriden to explicitly show in JSON.NET serialization.
        /// </remarks>
        [DataMember]
        public override string UserName
        {
            get => base.UserName;
            set => base.UserName = value;
        }

        [DataMember]
        public int AvatarId { get; set; }
        [DataMember]
        public UserFlag Flag { get; set; }
        
        public ICollection<Membership> Memberships { get; set; }
        public ICollection<User> blockedUsers { get; set; }

        [DataMember]
        public bool IsOnline { get; set; }
        
        [NotMapped]
        public IEnumerable<Channel> Channels => Memberships?.Select(m => m.Channel).ToList();

        public bool Equals(User other) => Id == other?.Id;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((User) obj);
        }

        public override int GetHashCode() => Id.GetHashCode() * 9;

        public static bool operator ==(User left, User right) => Equals(left, right);
        public static bool operator !=(User left, User right) => !Equals(left, right);
    }
}
