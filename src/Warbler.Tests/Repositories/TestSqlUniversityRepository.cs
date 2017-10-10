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
        private DbContextOptions<WarblerDbContext> Options { get; }
            = new DbContextOptionsBuilder<WarblerDbContext>()
                .UseInMemoryDatabase(nameof(TestSqlUniversityRepository))
                .Options;

        [TestMethod]
        public async Task CreateAsync_Should_Create_A_New_University()
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
        public void AllQueryable_Should_Return_Root_When_0_Is_Queried()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                IQueryable test = repo.AllQueryable(0);
                Assert.IsFalse(test == null);
            }
        }

        [TestMethod]
        public void AllQueryable_Should_Return_Server_Level_When_1_Is_Queried()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var query = new QueryDepth();
                query = (QueryDepth)1;
                IQueryable test = repo.AllQueryable(query);
                Assert.IsFalse(test == null);
            }
        }

        [TestMethod]
        public void AllQueryable_Should_Return_Channel_Level_When_2_Is_Queried()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var query = new QueryDepth();
                query = (QueryDepth)2;
                IQueryable test = repo.AllQueryable(query);
                Assert.IsFalse(test == null);
            }
        }

        [TestMethod]
        public void AllQueryable_Should_Return_User_Level_When_3_Is_Queried()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var query = new QueryDepth();
                query = (QueryDepth)3;
                IQueryable test = repo.AllQueryable(query);
                Assert.IsFalse(test == null);
            }
        }

        [TestMethod]
        public void AllQueryable_Should_Return_Message_Level_When_4_Is_Queried()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var query = new QueryDepth();
                query = (QueryDepth)4;
                IQueryable test = repo.AllQueryable(query);
                Assert.IsFalse(test == null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AllQueryable_Should_Fail_With_Unknown_QueryDepth()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlUniversityRepository(context);
                var query = new QueryDepth();
                query = (QueryDepth)5;
                IQueryable test = repo.AllQueryable(query);
            }
        }
    }
}
