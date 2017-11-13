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

        public async Task SubmitClaimAsync(ClaimRequest request)
            => await Repository.CreateAsync(request);
    }
}
