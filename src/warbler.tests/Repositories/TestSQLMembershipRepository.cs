using System.Linq;
using System.Threading.Tasks;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Search.NearBy.Response;
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
    public class TestSqlMembershipRepository
    {
        private DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlUniversityRepository))
                .Options;

        [TestInitialize]
        public async Task CreateUniversity()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlChannelRepository(context);
                var channel = new Channel()
                {
                    Name = "general",
                    Description = "Talk about anything.",
                    State = ChannelState.Active,
                    Type = ChannelType.Normal,
                    Memberships = new List<Membership>()
                };
                await repo.UpdateAsync(channel);
            }
        }

        [TestMethod]
        public void AllFor_Should_Return_List_Of_Channels_User_Is_Subscribed()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var user = new User
                {
                    UserName = "Bob"
                };

                var channel = context.Channels.FirstOrDefault(c => c.Name == "general");

                channel.Memberships.Add(new Membership { User = user });
            }

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMembershipRepository(context);
                Assert.AreEqual(context.Memberships.Include(m => m.User), repo.AllFor(context.Channels
                                        .FirstOrDefault(c => c.Name == "general")));
                                        
            }
        }
    }
}
