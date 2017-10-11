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

namespace Warbler.Tests.Repositories
{
    [TestClass]
    public class TestSqlUserRepository
    {
        private static DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlUserRepository))
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

                // Create a test university with default channels
                var testUniversity = await repo.CreateAsync(nearbyResult);

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
        public async Task IsNewAsync_Should_Be_False_If_User_Is_In_Any_Channel()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUserRepository(context);
                Assert.AreEqual(1, context.Memberships.Count());
                Assert.IsFalse(await repo.IsNewAsync(Bob));
            }
        }

        [TestMethod]
        public async Task IsNewAsync_Should_Be_True_If_User_Was_Recently_Created()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUserRepository(context);
                var jimmy = new User { UserName = "Jimmy" };
                Assert.IsTrue(await repo.IsNewAsync(jimmy));
            }
        }

        [TestMethod]
        public async Task SetOnlineAsync_Should_Change_IsOnline()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUserRepository(context);
                await repo.SetOnlineAsync(Bob, true);
            }

            Assert.IsTrue(Bob.IsOnline);
        }

        [TestMethod]
        public async Task FindByNameAsync_Should_Get_A_User_By_Username()
        {
            User foundUser;
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUserRepository(context);
                foundUser = await repo.FindByNameAsync("Bob");
            }

            Assert.AreEqual(Bob, foundUser);
        }
    }
}
