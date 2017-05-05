﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warbler.Areas.Chat.Interfaces;
using Warbler.Areas.Chat.Models;
using Warbler.Identity.Data;

namespace Warbler.Areas.Chat.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IUserRepository"/> for docs.
    /// </summary>
    public class SqlUserRepository : IUserRepository
    {
        private WarblerDbContext Context { get; }

        public SqlUserRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public async Task<bool> IsNewAsync(User user)
        {
            var membership = await Context.Memberships
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            return membership == null;
        }
    }
}
