using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IClaimRequestRepository"/> for docs.
    /// </summary>
    public class SqlClaimRequestRepository : IClaimRequestRepository
    {
        private WarblerDbContext Context { get; }

        public SqlClaimRequestRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public async Task CreateAsync(ClaimRequest claimRequest)
        {
            await Context.AddAsync(claimRequest);
            await Context.SaveChangesAsync();
        }
    }
}
