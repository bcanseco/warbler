using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warbler.Interfaces;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IAuthConfigRepository"/> for docs.
    /// </summary>
    public class SqlAuthConfigRepository : IAuthConfigRepository
    {
        private WarblerDbContext Context { get; }

        public SqlAuthConfigRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public async Task<AuthConfig> GetConfigAsync(University university)
            => await Context.AuthConfigs
                .Include(c => c.University)
                .Where(c => c.University.Equals(university))
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<List<AuthConfig>> GetConfigsAsync()
            => await Context.AuthConfigs.ToListAsync();

        public async Task SaveAsync(AuthConfig config)
        {
            Context.Update(config);
            await Context.SaveChangesAsync();
        }
    }
}
