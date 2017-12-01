using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warbler.Misc;
using Warbler.Services;
using Warbler.Models;
using Warbler.Models.Enums;
using System.Collections.Generic;
using Warbler.Interfaces;
using Warbler.Repositories;

namespace Warbler.Tests.Services
{
    [TestClass]
    public class TestUniversityService
    {
        private static DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestUniversityService))
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
        public async Task GetOrCreateAsync_Should_Return_A_Preexisting_Uni()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var test = new UniversityService(repo);
                var nearbyResult = new NearByResult
                {
                    Name = "Test",
                    Geometry = new Geometry { Location = new Location(0.0, 0.0) }
                };

                var templateService = new ChannelTemplateService(new SqlChannelTemplateRepository(context));
                await templateService.CreateDefaultTemplatesAsync();

                var uniTest = await test.GetOrCreateAsync(nearbyResult, await templateService.GetAsync());

                Assert.AreEqual(1, context.Universities.Count());
                Assert.AreEqual("Test", uniTest.Name);
            }
        }

        [TestMethod]
        [Ignore]
        public async Task GetOrCreateAsync_Should_Create_A_New_University()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var test = new UniversityService(repo);
                var nearbyResult = new NearByResult
                {
                    Name = "NewUni",
                    Geometry = new Geometry { Location = new Location(1.0, 1.0) }
                };

                var templateService = new ChannelTemplateService(new SqlChannelTemplateRepository(context));
                await templateService.CreateDefaultTemplatesAsync();

                var uniTest = await test.GetOrCreateAsync(nearbyResult, await templateService.GetAsync());

                //Assert.AreEqual(2, context.Universities.Count());
                Assert.AreEqual("NewUni", uniTest.Name);
            }
        }

        [TestMethod]
        public async Task JoinAsync_Should_Have_Users_Join_All_Channels_In_A_University()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var test = new UniversityService(repo);
                var newUser = new User { UserName = "Bill" };
                var testUni = await repo.LookupAsync
                                                    (context.Universities.FirstOrDefault
                                                    (u => u.Name == "Test").PlaceId);
                //Assert.AreEqual(2, context.Universities.Count());
                await test.JoinAsync(newUser, testUni);
                var testUser = context.Users.FirstOrDefault(u => u.UserName == "Bill");
                Assert.AreEqual(testUni.Server.Channels.Count(), testUser.Channels.Count());
            }
        }

        [TestMethod]
        public async Task AllQueryable_Message_Should_Get_Everything()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var testUni = new UniversityService(repo);
                var universities = await testUni.GetAllAsync();

                Assert.IsNotNull(universities.First());
                Assert.IsNotNull(universities.First().Server);
                Assert.IsNotNull(universities.First().Server.Channels);
                Assert.IsNotNull(universities.First().Server.Channels.First().Users);
                Assert.IsNotNull(universities.First().Server.Channels.First().Messages);
            }
        }
    }
}
