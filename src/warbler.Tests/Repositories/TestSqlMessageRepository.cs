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
    public class TestSqlMessageRepository
    {
        private DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlChannelRepository))
                .Options;

        private User Bob { get; set; }
        private Channel General { get; set; }

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
        public async Task CreateAsync_should_return_a_Message_With_A_NonNull_Value()
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
        public async Task LatestIn_Should_Return_The_Last_25_Messages_Submitted_In_Reverse_Chronological_Order()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMessageRepository(context);

                for (int i = 0; i < 25; i++)
                {
                    await repo.CreateAsync(i.ToString(), Bob, General);
                }
            }

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlMessageRepository(context);
                var messages = repo.LatestIn(General);
                var firstMessage = messages.First().Result;
                var lastMessage = messages.Last().Result;

                Assert.AreEqual("24", lastMessage.Text);
                Assert.AreEqual("0", firstMessage.Text);
                Assert.IsTrue(0 < System.DateTime.Compare(
                              lastMessage.SendDate, firstMessage.SendDate));
            }
        }
    }
}
