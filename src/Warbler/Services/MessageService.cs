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
        public async Task<List<Message>> LatestIn(Channel channel, List<User> blockedUser)
            => await Repository.LatestIn(channel, blockedUser).ToList();

        /// <summary>
        ///   Creates and returns a message.
        /// </summary>
        /// <param name="text">The content of the message.</param>
        /// <param name="user">The sender.</param>
        /// <param name="channel">The containing channel.</param>
        /// <returns>The created message.</returns>
        public async Task<Message> CreateAsync(string text, User user, Channel channel)
            => await Repository.CreateAsync(text, user, channel);
    }
}
