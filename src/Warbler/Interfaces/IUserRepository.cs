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

        /// <summary>
        ///   Returns the <see cref="User"/> matching the given username.
        /// </summary>
        /// <param name="username">The username to search with.</param>
        /// <returns>A User object.</returns>
        Task<User> FindByNameAsync(string username);

        /// <summary>
        ///   Adds the <see cref="User"/> matching the given username to block list.
        /// </summary>
        /// <param name="blockedUser">The username to search with.</param>
        Task<User> AddBlockedUser(User user, string blockedUser);

        /// <summary>
        ///   Sets <see cref="User.IsOnline"/>.
        /// </summary>
        /// <param name="user">The user to manipulate.</param>
        /// <param name="isOnline">True to set as online, false otherwise.</param>
        Task SetOnlineAsync(User user, bool isOnline);
    }
}
