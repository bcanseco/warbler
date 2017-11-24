using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    public interface IClaimRequestRepository
    {
        /// <summary>
        ///   Saves the provided claim request to the database.
        /// </summary>
        /// <param name="claimRequest">The claim request form model.</param>
        Task CreateAsync(ClaimRequest claimRequest);

        /// <summary>
        ///   Gets all claim requests in the database.
        /// </summary>
        /// <returns>A list of claim requests.</returns>
        Task<List<ClaimRequest>> GetAllAsync();

        /// <summary>
        ///   Gets all claim requests submitted by a user.
        /// </summary>
        /// <param name="user">The user to search with.</param>
        /// <returns>A list of claim requests.</returns>
        Task<List<ClaimRequest>> AllFromUserAsync(User user);

        /// <summary>
        ///   Saves changes to the claim request in the database.
        /// </summary>
        /// <param name="request">The changed claim request.</param>
        Task UpdateAsync(ClaimRequest request);
    }
}
