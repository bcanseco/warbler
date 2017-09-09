using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Warbler.Exceptions;
using Warbler.Misc;
using Warbler.Repositories;
using Warbler.Models;
using Warbler.Models.Enums;
using System.Collections.Generic;

namespace Warbler.Tests.Repositories
{
    [TestClass]
    public class TestSQLChannelRepository
    {
        private DbContextOptions<WarblerDbContext> Options { get; }
               = new DbContextOptionsBuilder<WarblerDbContext>()
                   .UseInMemoryDatabase(nameof(TestSqlUniversityRepository))
                   .Options;

        [TestMethod]
        public async Task UpdateAsync_Should_Update_Channel()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlChannelRepository(context);

                Channel testChannel = new Channel
                {
                    Name = "general",
                    Description = "",
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
                Assert.AreEqual("", context.Channels
                                         .Where(c => c.Name == "general")
                                         .FirstOrDefault().Description);
            }
        }
    }
}
