using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of university claim request forms.
    /// </summary>
    public class ClaimRequestService
    {
        private IClaimRequestRepository Repository { get; }

        public ClaimRequestService(IClaimRequestRepository repository)
            => Repository = repository;

        public async Task SubmitAsync(ClaimRequest request)
            => await Repository.CreateAsync(request);

        public async Task<List<ClaimRequest>> GetAllUnresolvedAsync()
            => (await Repository.GetAllAsync())
                .Where(r => r.IsAccepted == null)
                .ToList();

        /// <summary>
        ///   Returns the first (and hopefully only) university a user
        ///   has claimed, or null if none exist.
        /// </summary>
        /// <param name="user">The user to search with.</param>
        /// <returns>A university or null.</returns>
        public async Task<University> GetClaimedUniversityAsync(User user)
            => (await Repository.AllFromUserAsync(user))
                .Where(r => r.IsAccepted ?? false)
                .Select(r => r.University)
                .FirstOrDefault();

        public async Task UpdateAsync(ClaimRequest request)
            => await Repository.UpdateAsync(request);
    }
}
