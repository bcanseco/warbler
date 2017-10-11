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
using Warbler.Models.Enums;
using System;

namespace Warbler.Tests.Repositories
{
    [TestClass]
    public class TestSqlUniversityRepository
    {
        private DbContextOptions<WarblerDbContext> Options { get; set; }
         
        [TestInitialize]
        public void SetUpDatabase()
        {
            Options = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlUniversityRepository))
                .Options;
        }

        private async Task SetUpSampleUniversity()
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
        public async Task CreateAsync_Should_Create_A_New_University()
        {
            await SetUpSampleUniversity();

            // Use separate context instance to verify correct data was saved to DB
            using (var context = new WarblerDbContext(Options))
            {
                Assert.AreEqual(1, context.Universities.Count());
                Assert.AreEqual("Test", context.Universities.Single().Name);
            }
        }

        [TestMethod]
        public async Task SaveASync_Should_Save_Changes_On_Context()
        {
            var saveChangesCalled = false;

            // Create a fake WarblerDbContext
            var mockContext = new Mock<WarblerDbContext>(Options);

            // Watch its SaveChangesAsync() method to see if it gets called by the repo
            mockContext.Setup(x => x.SaveChangesAsync(default(CancellationToken)))
                .Callback(() => saveChangesCalled = true)
                .ReturnsAsync(0);

            // Attach it to the repo and call SaveAsync()
            var repo = new SqlUniversityRepository(mockContext.Object);
            await repo.SaveAsync();

            Assert.IsTrue(saveChangesCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(UniversityNotFoundException))]
        public async Task LookupAsync_Should_Fail_With_Unknown_PlaceId()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                await repo.LookupAsync("Test");
            }
        }

        [TestMethod]
        public async Task AllQueryable_University_Should_Not_Get_Servers()
        {
            await SetUpSampleUniversity();

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var universities = repo
                    .AllQueryable(QueryDepth.University)
                    .ToList();

                Assert.IsNull(universities.First().Server);
            }
        }

        [TestMethod]
        public async Task AllQueryable_Server_Should_Not_Get_Channels()
        {
            await SetUpSampleUniversity();

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var universities = repo
                    .AllQueryable(QueryDepth.Server)
                    .ToList();

                Assert.IsNotNull(universities.First().Server);
                Assert.IsNull(universities.First().Server.Channels);
            }
        }

        [TestMethod]
        public async Task AllQueryable_Channel_Should_Not_Get_Users_Messages()
        {
            await SetUpSampleUniversity();

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var universities = repo
                    .AllQueryable(QueryDepth.Channel)
                    .ToList();
                var channel = universities.First().Server.Channels.First();

                Assert.IsNotNull(channel);
                Assert.IsNull(channel.Users);
                Assert.IsNull(channel.Messages);
            }
        }

        [TestMethod]
        public async Task AllQueryable_User_Should_Not_Get_Messages()
        {
            await SetUpSampleUniversity();

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var universities = repo
                    .AllQueryable(QueryDepth.User)
                    .ToList();
                var channel = universities.First().Server.Channels.First();

                Assert.IsNotNull(channel.Users);
                Assert.IsNull(channel.Messages);
            }
        }

        [TestMethod]
        public async Task AllQueryable_Message_Should_Get_Everything()
        {
            await SetUpSampleUniversity();

            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var universities = repo
                    .AllQueryable(QueryDepth.Message)
                    .ToList();

                Assert.IsNotNull(universities.First());
                Assert.IsNotNull(universities.First().Server);
                Assert.IsNotNull(universities.First().Server.Channels);
                Assert.IsNotNull(universities.First().Server.Channels.First().Users);
                Assert.IsNotNull(universities.First().Server.Channels.First().Messages);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AllQueryable_Should_Fail_With_Unknown_QueryDepth()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                repo.AllQueryable((QueryDepth)5);
            }
        }
    }
}
