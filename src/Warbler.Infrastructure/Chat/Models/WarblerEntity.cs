using System;

namespace Warbler.Infrastructure.Chat.Models
{
    /// <summary>
    ///   The base class every Warbler model derives from.
    ///   Provides equality comparers based on entity ids.
    /// </summary>
    public abstract class WarblerEntity : IEquatable<WarblerEntity>
    {
        protected int Id { get; }

        /// <summary>
        ///   Constructs an entity instance with a unique id.
        /// </summary>
        /// <param name="id">The id to use.</param>
        protected WarblerEntity(int id)
        {
            Id = id;
        }

        public override int GetHashCode()
            => Id;

        public bool Equals(WarblerEntity other)
            => Id == other.Id;

        public static bool operator ==(WarblerEntity a, WarblerEntity b)
            => a.Id == b.Id;

        public static bool operator !=(WarblerEntity a, WarblerEntity b)
            => !(a == b);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((WarblerEntity)obj);
        }
    }
}