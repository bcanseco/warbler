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
using Warbler.Services;

namespace Warbler.Tests.Repositories
{
    [TestClass]
    public class TestSqlMembershipRepository
    {
        private static DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlMembershipRepository))
                .Options;

        private static User Bob { get; set; }
        private static Channel General { get; set; }

        [ClassInitialize]
        public static async Task CreateUniversity(TestContext _)
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var nearbyResult = new NearByResult
                {
                    Name = "Test",
                    Geometry = new Geometry { Location = new Location(0.0, 0.0) }
                };
                var templateService = new ChannelTemplateService(new SqlChannelTemplateRepository(context));
                await templateService.CreateDefaultTemplatesAsync();

                // Create a test university with default channels
                var testUniversity = await repo.CreateAsync(nearbyResult, await templateService.GetAsync());

                // Save a reference to one of the default channels and a new user
                General = testUniversity.Server.Channels.Single(ch => ch.Name == "general");
                Bob = new User { UserName = "Bob" };

                // Add a test user to it
                General.Memberships.Add(new Membership { User = Bob });

                // Save it to the in-memory database for use in tests
                await repo.SaveAsync();
            }
        }

        [TestMethod]
        public async Task AllForUser_Should_Return_Channels_Where_User_Is_Member()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMembershipRepository(context);
                var bobMemberships = await repo.AllFor(Bob).ToList();

                // Bob was only added to one channel (#general)
                Assert.AreEqual(1, bobMemberships.Count);
                Assert.IsTrue(bobMemberships.Single().Channel.Name == "general");
            }
        }

        [TestMethod]
        public async Task AllForChannel_Should_Return_Users_That_Are_Members()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMembershipRepository(context);
                var generalMemberships = await repo.AllFor(General).ToList();

                // General only had one member added (Bob)
                Assert.AreEqual(1, generalMemberships.Count);
                Assert.IsTrue(generalMemberships.Single().User.UserName == "Bob");
            }
        }
    }
}
