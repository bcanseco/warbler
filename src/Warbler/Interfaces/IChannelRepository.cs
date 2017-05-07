using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    /// <summary>
    ///   Defines an API for message-based queries against a repository.
    /// </summary>
    public interface IChannelRepository
    {
        /// <summary>
        ///   Updates the properties of a channel in the database.
        /// </summary>
        /// <param name="channel">The channel to update.</param>
        Task UpdateAsync(Channel channel);
    }
}
