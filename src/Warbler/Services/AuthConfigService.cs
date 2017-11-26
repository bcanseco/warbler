using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of auth configs.
    /// </summary>
    public class AuthConfigService
    {
        private IAuthConfigRepository Repository { get; }

        public AuthConfigService(IAuthConfigRepository repository)
        {
            Repository = repository;
        }

        public async Task<AuthConfig> GetConfigAsync(University university)
            => await Repository.GetConfigAsync(university);

        public async Task SaveAsync(AuthConfig config)
            => await Repository.SaveAsync(config);
    }
}
