using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    /// <summary>
    ///   Defines an API for membership-based queries against a repository.
    /// </summary>
    public interface IMembershipRepository
    {
        /// <summary>
        ///   Retrieves a list of memberships that the given user is part of.
        /// </summary>
        /// <param name="user">The user to retrieve memberships for.</param>
        /// <returns>
        ///   A list of Membership objects filled-in up to the university level.
        /// </returns>
        IAsyncEnumerable<Membership> AllFor(User user);

        /// <summary>
        ///   Retrieves a list of memberships that the given channel contains.
        /// </summary>
        /// <param name="channel">The channel to retrieve memberships for.</param>
        /// <returns>
        ///   A list of Membership objects filled-in up to the university level.
        /// </returns>
        IAsyncEnumerable<Membership> AllFor(Channel channel);

        /// <summary>
        ///   Removes all members of a channel.
        /// </summary>
        /// <param name="channel">The channel whose members will be removed.</param>
        Task DropMembershipsAsync(Channel channel);
    }
}
