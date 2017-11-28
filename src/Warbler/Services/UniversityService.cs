using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.EntityFrameworkCore;
using Warbler.Exceptions;
using Warbler.Interfaces;
using Warbler.Models;
using Warbler.Models.Enums;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of university data.
    /// </summary>
    public class UniversityService
    {
        private IUniversityRepository Repository { get; }

        public UniversityService(IUniversityRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Gets the university associated with the given
        ///   Google Places result or creates it if nonexistent.
        /// </summary>
        /// <param name="uni">The Google Places search result to use.</param>
        /// <returns>The related University object.</returns>
        public async Task<University> GetOrCreateAsync(NearByResult uni)
        {
            try
            {
                return await Repository.LookupAsync(uni.PlaceId);
            }
            catch (UniversityNotFoundException)
            {
                return await Repository.CreateAsync(uni);
            }
        }

        /// <summary>
        ///   Adds a user as a member to every channel in a university's server.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <param name="university">The university whose channels will be added to.</param>
        /// <param name="samlName">Used for claimed universities.</param>
        /// <remarks>
        ///   This will create new rows in the Membership table (User x Channel).
        /// </remarks>
        public async Task JoinAsync(User user, University university, string samlName = null)
        {
            // Filter out any channels that a user is already a member of
            var channels = university.Server.Channels
                .Where(ch => !ch.Memberships.Any(m => m.User.Equals(user)));
                
            foreach (var channel in channels)
                channel.Memberships.Add(new Membership {User = user, SamlName = samlName});

            await Repository.SaveAsync();
        }

        /// <summary>
        ///   Gets a list of all universities and their properties
        ///   down to the message level. Results are untracked.
        /// </summary>
        public async Task<List<University>> GetAllAsync()
            => await Repository.AllQueryable(QueryDepth.Message)
                .AsNoTracking()
                .ToListAsync();

        public async Task ApplyClaimAsync(University university, string submitterId)
            => await Repository.ApplyClaimAsync(university, submitterId);

        public async Task<University> FindByIdAsync(int id)
            => await Repository.AllQueryable(QueryDepth.User)
                .FirstAsync(u => u.Id == id);
    }
}
