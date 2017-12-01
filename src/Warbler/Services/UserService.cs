using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of user data.
    /// </summary>
    public class UserService
    {
        private IUserRepository Repository { get; }

        public UserService(IUserRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Returns <see langword="true"/> if the user is not a
        ///   member of any channel, <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="user">The user to check.</param>
        public async Task<bool> IsNewAsync(User user)
            => await Repository.IsNewAsync(user);

        /// <remarks>
        ///   Used instead of UserManager's method intentionally due to weird
        ///   task cancellation errors (probably a Kestrel thing).
        /// </remarks>
        public async Task<User> FindByNameAsync(string username)
            => await Repository.FindByNameAsync(username);

        /// <summary>
        ///   Sets a user's isOnline property.
        /// </summary>
        /// <param name="user">The user to edit.</param>
        /// <param name="isOnline">True to set online, false to set offline.</param>
        public async Task SetOnlineAsync(User user, bool isOnline = true)
            => await Repository.SetOnlineAsync(user, isOnline);
    }
}
