using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    /// <summary>
    ///   Defines an API for message-based queries against a repository.
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        ///   Gets the 25 latest saved Message objects for a Channel.
        /// </summary>
        /// <param name="channel">The channel to fetch messages from.</param>
        /// <returns>Up to 25 stored Message objects.</returns>
        IAsyncEnumerable<Message> LatestIn(Channel channel);
    }
}
