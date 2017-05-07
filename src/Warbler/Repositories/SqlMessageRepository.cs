using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Warbler.Interfaces;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IMessageRepository"/> for docs.
    /// </summary>
    public class SqlMessageRepository : IMessageRepository
    {
        private WarblerDbContext Context { get; }

        public SqlMessageRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public IAsyncEnumerable<Message> LatestIn(Channel channel)
            => Context.Channels
                .Include(ch => ch.Messages)
                .Single(ch => ch.Equals(channel))
                .Messages.OrderByDescending(m => m.SendDate)
                .Take(25)
                .ToAsyncEnumerable();
    }
}
