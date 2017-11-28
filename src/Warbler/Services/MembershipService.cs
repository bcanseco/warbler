using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of membership data.
    /// </summary>
    public class MembershipService
    {
        private IMembershipRepository Repository { get; }

        public MembershipService(IMembershipRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Returns a collection of channels that the user is a member of.
        ///   Channels include filled-in server and university props.
        /// </summary>
        /// <param name="user">The user to match against.</param>
        public async Task<ICollection<Channel>> AllChannelsForAsync(User user)
            => await Repository.AllFor(user)
                .Select(m => m.Channel)
                .ToList();

        /// <summary>
        ///   Returns a collection of memberships belonging to a channel.
        /// </summary>
        /// <param name="channel">The channel to fetch memberships for.</param>
        public async Task<ICollection<Membership>> AllMembershipsForAsync(Channel channel)
            => await Repository.AllFor(channel).ToList();

        /// <summary>
        ///   Returns a collection of memberships belonging to a user.
        /// </summary>
        /// <param name="user">The user to fetch memberships for.</param>
        public async Task<ICollection<Membership>> AllMembershipsForAsync(User user)
            => await Repository.AllFor(user).ToList();

        public async Task DropMembershipsAsync(Channel channel)
            => await Repository.DropMembershipsAsync(channel);
    }
}
