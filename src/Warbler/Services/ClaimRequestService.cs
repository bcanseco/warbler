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

        public async Task UpdateAsync(ClaimRequest request)
            => await Repository.UpdateAsync(request);
    }
}
