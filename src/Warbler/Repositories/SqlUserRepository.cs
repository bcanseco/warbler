using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warbler.Interfaces;
using Warbler.Models;
using Warbler.Misc;

namespace Warbler.Repositories
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


        public async Task SetOnlineAsync(User user, bool isOnline)
        {
            user.IsOnline = isOnline;
            await Context.SaveChangesAsync();
        }

        public async Task AddBlockedUser(User user, string blockedUser)
        {
            var block = await Context.Users.FirstOrDefaultAsync(u => u.UserName == blockedUser);
            user.blockedUsers.Add(block);
            await Context.SaveChangesAsync();
        }

        public async Task<User> FindByNameAsync(string userName)
            => await Context.Users.SingleAsync(u => u.UserName == userName);
    }
}
