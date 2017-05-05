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
    }
}
