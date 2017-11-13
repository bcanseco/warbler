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
    }
}
