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
    public class TestSqlMessageRepository
    {
        private static DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlMessageRepository))
                .Options;

        private User Bob { get; set; }
        private Channel General { get; set; }

        [ClassInitialize]
        public static async Task CreateTemplates(TestContext _)
        {
            using (var context = new WarblerDbContext(Options))
            {
                await new ChannelTemplateService(new SqlChannelTemplateRepository(context))
                    .CreateDefaultTemplatesAsync();
            }
        }

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

                var templateService = new ChannelTemplateService(new SqlChannelTemplateRepository(context));

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
        public async Task CreateAsync_Should_Create_A_New_Message()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMessageRepository(context);

                var message = await repo.CreateAsync("Hello", Bob, General);

                Assert.AreEqual(1, context.Messages.Count());
                Assert.IsNotNull(message);
                Assert.AreEqual("Hello", message.Text);
                Assert.AreEqual(Bob.Id, message.UserId);
                Assert.AreEqual(General.Id, message.ChannelId);
            }
        }

        [TestMethod]
        public async Task LatestIn_Should_Return_Messages_In_Reverse_Chronological_Order()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMessageRepository(context);

                await repo.CreateAsync("foo", Bob, General);
                await repo.CreateAsync("bar", Bob, General);
            }

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMessageRepository(context);
                var messages = repo.LatestIn(General, Bob.blockedUsers.ToList());
                var firstMessage = messages.First().Result;
                var lastMessage = messages.Last().Result;

                Assert.AreEqual("bar", lastMessage.Text);
                Assert.AreEqual("foo", firstMessage.Text);
                Assert.IsTrue(lastMessage.SendDate > firstMessage.SendDate);
            }
        }
    }
}
