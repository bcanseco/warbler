using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    public interface IChannelTemplateRepository
    {
        /// <summary>
        ///   Updates the properties of a channelTemplate in the database.
        /// </summary>
        /// <param name="channelTemplate">The channelTemplate to update.</param>
        Task UpdateAsync(ChannelTemplate channelTemplate);
    }
}
