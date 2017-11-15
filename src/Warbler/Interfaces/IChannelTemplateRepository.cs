using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    public interface IChannelTemplateRepository
    {
        /// <summary>
        ///   Adds a new channel template to the database.
        /// </summary>
        /// <param name="channelTemplate">The template to create.</param>
        /// <param name="saveChanges">
        ///   True to have this call also save all templates
        ///   that have been added to the context (default: false)
        /// </param>
        Task CreateAsync(ChannelTemplate channelTemplate, bool saveChanges = false);

        /// <summary>
        ///   Retrieves all channel templates from the database.
        /// </summary>
        /// <returns>A list of channel templates.</returns>
        Task<List<ChannelTemplate>> GetAsync();
    }
}
