using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of channel data.
    /// </summary>
    public class ChannelService
    {
        private IChannelRepository Repository { get; }

        public ChannelService(IChannelRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Updates a channel's properties (like messages and users).
        /// </summary>
        /// <param name="channel">The channel to update.</param>
        public async Task UpdateAsync(Channel channel)
            => await Repository.UpdateAsync(channel);
    }
}
