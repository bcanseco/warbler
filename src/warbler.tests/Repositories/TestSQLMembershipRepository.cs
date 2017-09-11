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
                var repo = new SqlUniversityRepository(context);
                var nearbyResult = new NearByResult
                {
                    Name = "Test",
                    Geometry = new Geometry { Location = new Location(0.0, 0.0) }
                };

                await repo.CreateAsync(nearbyResult);
            }
        }

        [TestMethod]
        public async Task AllFor_Should_Return_List_Of_Channels_User_Is_Subscribed()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var user = new User
                {
                    UserName = "Bob"
                };

                var channels = context.Channels.Include(ch => ch.Memberships);

                channels.FirstOrDefault(c => c.Name == "general").Memberships.Add(new Membership { User = user });
            }

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMembershipRepository(context);
                Assert.AreEqual(1, context.Memberships.Count());
                Assert.AreEqual(context.Memberships.ToList(), await repo.AllFor(context.Channels
                                                .FirstOrDefault(c => c.Name == "general")).ToList());
                                        
            }
        }

        [TestMethod]
        public void AllFor_Should_Return_List_Of_Users_Subscribed_To_Channel()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var user = new User
                {
                    UserName = "Bob"
                };

                var channels = context.Channels.Include(ch => ch.Memberships);

                channels.FirstOrDefault(c => c.Name == "general").Memberships.Add(new Membership { User = user });
            }

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMembershipRepository(context);
                Assert.AreEqual(context.Memberships.FirstOrDefault(u => u.User.UserName == "Bob").Channel.Name, repo.AllFor(new User { UserName = "Bob"}));

            }
        }
    }
}
