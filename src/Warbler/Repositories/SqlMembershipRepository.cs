using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Warbler.Interfaces;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IMembershipRepository"/> for docs.
    /// </summary>
    public class SqlMembershipRepository : IMembershipRepository
    {
        private WarblerDbContext Context { get; }

        public SqlMembershipRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public IAsyncEnumerable<Membership> AllFor(User user)
            => BaseQuery()
                .Where(m => m.User.Equals(user))
                .ToAsyncEnumerable();

        public IAsyncEnumerable<Membership> AllFor(Channel channel)
            => BaseQuery()
                .Where(m => m.Channel.Equals(channel))
                .ToAsyncEnumerable();

        private IIncludableQueryable<Membership, University> BaseQuery()
            => Context.Memberships
                .Include(m => m.User)
                .Include(m => m.Channel)
                    .ThenInclude(ch => ch.Server)
                        .ThenInclude(srv => srv.University);
    }
}
