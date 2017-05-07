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
        ///   Returns a list of memberships that the channel contains.
        /// </summary>
        /// <param name="channel">The channel to fetch for.</param>
        /// <returns></returns>
        public async Task<ICollection<Membership>> AllForAsync(Channel channel)
            => await Repository.AllFor(channel).ToList();

        /// <summary>
        ///   Returns a list of channels that the user is a member of.
        ///   Channels include filled-in server and university props.
        /// </summary>
        /// <param name="user">The user to match against.</param>
        public async Task<List<Channel>> AllChannelsForAsync(User user)
            => await Repository.AllFor(user)
                .Select(m => m.Channel)
                .ToList();
    }
}
