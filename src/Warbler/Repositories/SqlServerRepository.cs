using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IServerRepository"/> for docs.
    /// </summary>
    public class SqlServerRepository : IServerRepository
    {
        private WarblerDbContext Context { get; }

        public SqlServerRepository(WarblerDbContext context)
            => Context = context;

        public async Task EnableAuthAsync(Server server)
        {
            server.IsAuthEnabled = true;
            await Context.SaveChangesAsync();
        }
    }
}
