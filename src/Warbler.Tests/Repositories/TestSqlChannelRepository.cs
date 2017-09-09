using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warbler.Misc;
using Warbler.Repositories;
using Warbler.Models;
using Warbler.Models.Enums;
using System.Collections.Generic;

namespace Warbler.Tests.Repositories
{
    [TestClass]
    public class TestSqlChannelRepository
    {
        private DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlChannelRepository))
                .Options;

        [TestMethod]
        public async Task UpdateAsync_Should_Update_Channel()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlChannelRepository(context);

                var testChannel = new Channel
                {
                    Name = "general",
                    Description = string.Empty,
                    State = ChannelState.Active,
                    Type = ChannelType.Normal,
                    Memberships = new List<Membership>()
                };

                await repo.UpdateAsync(testChannel);
            }

            // Use separate context instance to verify correct data was saved to DB
            using (var context = new WarblerDbContext(Options))
            {
                Assert.AreEqual(1, context.Channels.Count());
                Assert.AreEqual(string.Empty, context.Channels
                    .FirstOrDefault(c => c.Name == "general").Description);
            }
        }
    }
}
