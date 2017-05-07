using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    /// <summary>
    ///   Defines an API for user-based queries against a repository.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        ///   Returns <see langword="true"/> if no memberships exist with
        ///   the id of the given user, <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="user">The user whose ID will be matched against.</param>
        Task<bool> IsNewAsync(User user);
    }
}
