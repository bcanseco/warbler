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

namespace Warbler.Tests.Services
{
    [TestClass]
    public class TestClaimRequestService
    {
        private static DbContextOptions<WarblerDbContext> Options { get; }
           = new DbContextOptionsBuilder<WarblerDbContext>()
               .UseInMemoryDatabase(nameof(TestClaimRequestService))
               .Options;

        public static University Test { get; set; }
        private static User Bob { get; set; }

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
                Test = testUniversity;
                Bob = new User { UserName = "Bob" };
                // Save it to the in-memory database for use in tests
                await repo.SaveAsync();
            }
        }

        [TestMethod]
        [Ignore]
        public async Task SubmitClaimAsync_Should_Create_New_Claim_Request()
        {
            using (var context = new WarblerDbContext(Options))
            {
                var repo = new SqlClaimRequestRepository(context);
                var test = new ClaimRequestService(repo);
                
               
                var request = new ClaimRequest
                {
                    FirstName = "William",
                    LastName = "Harris",
                    University = Test,
                    PositionTitle = "Admin",
                    Comments = "none",
                    Submitter = Bob
                };

                await test.SubmitAsync(request);
            }

            using (var context = new WarblerDbContext(Options))
            {
                var claimRequest = context.ClaimRequests.Single();
                Assert.AreEqual(1, context.ClaimRequests.Count());
                Assert.AreEqual("William", claimRequest.FirstName);
                Assert.AreEqual("Harris", claimRequest.LastName);
                Assert.AreEqual(Test, claimRequest.University);
                Assert.AreEqual("Admin", claimRequest.PositionTitle);
                Assert.AreEqual("none", claimRequest.Comments);
            }
        }
    }
}
