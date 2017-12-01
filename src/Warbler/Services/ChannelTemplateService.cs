using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of channel templates.
    /// </summary>
    public class ChannelTemplateService
    {
        private IChannelTemplateRepository Repository { get; }

        public ChannelTemplateService(IChannelTemplateRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Loads the database up with a default set of channel templates.
        /// </summary>
        public async Task CreateDefaultTemplatesAsync()
        {
            await Repository.CreateAsync(new ChannelTemplate("general", "Talk about anything."));
            await Repository.CreateAsync(new ChannelTemplate("biology", "Biology discussion."));
            await Repository.CreateAsync(new ChannelTemplate("compsci", "Computer science news and help."));
            await Repository.CreateAsync(new ChannelTemplate("math", "2 + 2 = 4"));
            await Repository.CreateAsync(new ChannelTemplate("psych", "Discuss the psychology curriculum."));
            await Repository.CreateAsync(new ChannelTemplate("physics", "What goes up must come down."));
            await Repository.CreateAsync(new ChannelTemplate("law", "Lady liberty's channel"), true);
        }

        public async Task<List<ChannelTemplate>> GetAsync() => await Repository.GetAsync();
    }
}
