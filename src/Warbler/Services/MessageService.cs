using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of message data.
    /// </summary>
    public class MessageService
    {
        private IMessageRepository Repository { get; }

        public MessageService(IMessageRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Gets the latest saved messages for a channel.
        /// </summary>
        /// <param name="channel">The channel to fetch messages from.</param>
        /// <returns>A collection of saved messages.</returns>
        public async Task<List<Message>> LatestIn(Channel channel)
            => await Repository.LatestIn(channel).ToList();
    }
}
