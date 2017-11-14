using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    public class ChannelTemplateService
    {
        private IChannelTemplateRepository Repository { get; }

        public ChannelTemplateService(IChannelTemplateRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Updates a channel's properties (like messages and users).
        /// </summary>
        /// <param name="channel">The channel to update.</param>
        public async Task UpdateAsync(ChannelTemplate channelTemplate)
            => await Repository.UpdateAsync(channelTemplate);
    }
}
