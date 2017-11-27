using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    /// <summary>
    ///   Defines an API for third-party authentication configurations.
    /// </summary>
    public interface IAuthConfigRepository
    {
        /// <summary>
        ///   Returns a university's authentication configuration or null.
        /// </summary>
        /// <param name="university">The claimed university to match.</param>
        /// <returns>The auth config, or null if none exists.</returns>
        Task<AuthConfig> GetConfigAsync(University university);

        /// <summary>
        ///   Gets all authentication configurations from the database.
        /// </summary>
        /// <returns>A list of auth configs.</returns>
        Task<List<AuthConfig>> GetConfigsAsync();

        /// <summary>
        ///   Adds or updates an authentication configuration to the database.
        /// </summary>
        /// <param name="config">The configuration to save.</param>
        Task SaveAsync(AuthConfig config);
    }
}
