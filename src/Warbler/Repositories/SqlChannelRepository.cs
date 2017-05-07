using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IChannelRepository"/> for docs.
    /// </summary>
    public class SqlChannelRepository : IChannelRepository
    {
        private WarblerDbContext Context { get; }

        public SqlChannelRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public async Task UpdateAsync(Channel channel)
        {
            Context.Update(channel);
            await Context.SaveChangesAsync();
        }
    }
}
