using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IChannelTemplateRepository"/> for docs.
    /// </summary>
    public class SqlChannelTemplateRepository : IChannelTemplateRepository
    {
        private WarblerDbContext Context { get; }

        public SqlChannelTemplateRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public async Task CreateDefaultChannel()
        {
            var channelTemplate = new ChannelTemplate
            {
                Name = "General",
                Description = "Talk about anything."
            };
            Context.ChannelTemplates.Add(channelTemplate);
            await Context.SaveChangesAsync();
        }

        public async Task CreateChannelTemplate(string name, string description)
        {
            var channelTemplate = new ChannelTemplate
            {
                Name = name,
                Description = description
            };
            Context.ChannelTemplates.Add(channelTemplate);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChannelTemplate channelTemplate)
        {
            Context.Update(channelTemplate);
            await Context.SaveChangesAsync();
        }
    }
}
