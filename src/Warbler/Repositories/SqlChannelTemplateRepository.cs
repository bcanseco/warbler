using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task CreateAsync(ChannelTemplate channelTemplate, bool saveChanges = false)
        {
            await Context.AddAsync(channelTemplate);
            if (saveChanges)
            {
                await Context.SaveChangesAsync();
            }
        }

        public async Task<List<ChannelTemplate>> GetAsync()
            => await Context.ChannelTemplates.ToListAsync();
    }
}
