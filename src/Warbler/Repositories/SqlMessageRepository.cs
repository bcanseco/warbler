using System;
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
    ///   Look at <see cref="IMessageRepository"/> for docs.
    /// </summary>
    public class SqlMessageRepository : IMessageRepository
    {
        private WarblerDbContext Context { get; }

        public SqlMessageRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public IAsyncEnumerable<Message> LatestIn(Channel channel, List<User> blockedUsers)
            => Context.Channels
                .Include(ch => ch.Messages)
                .Single(ch => ch.Equals(channel)).Messages
                .OrderBy(m => m.SendDate)
                .Where(m => !blockedUsers.Contains(m.Sender))
                .Take(25)
                .ToAsyncEnumerable();

        public async Task<Message> CreateAsync(string text, User user, Channel channel)
        {
            var message = new Message
            {
                Text = text,
                UserId = user.Id,
                ChannelId = channel.Id,
                SendDate = DateTime.Now
            };

            message = (await Context.Messages.AddAsync(message)).Entity;
            await Context.SaveChangesAsync();
            return message;
        }
    }
}
