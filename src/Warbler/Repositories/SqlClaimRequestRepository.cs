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

        public async Task<List<ClaimRequest>> GetAllAsync()
            => await Context.ClaimRequests
                .Include(r => r.University)
                    .ThenInclude(u => u.Server)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<ClaimRequest>> AllFromUserAsync(User user)
            => await Context.ClaimRequests
                .Where(r => r.Submitter.Equals(user))
                .Include(r => r.University)
                .ToListAsync();

        public async Task UpdateAsync(ClaimRequest claimRequest)
        {
            Context.Update(claimRequest);
            await Context.SaveChangesAsync();
        }
    }
}
