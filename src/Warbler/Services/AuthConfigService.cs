using System.Linq;
using System.Threading.Tasks;
using ComponentSpace.Saml2.Configuration;
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
        private SamlConfigurations LoadedConfigs { get; }

        public AuthConfigService(IAuthConfigRepository repository, SamlConfigurations configs)
        {
            Repository = repository;
            LoadedConfigs = configs;
        }

        public async Task<AuthConfig> GetConfigAsync(University university)
            => await Repository.GetConfigAsync(university);

        public async Task SaveAsync(AuthConfig config)
        {
            await Repository.SaveAsync(config);
            await RefreshConfigsAsync();
        }

        public async Task RefreshConfigsAsync()
        {
            LoadedConfigs.Configurations.First().PartnerIdentityProviderConfigurations
                = (await Repository.GetConfigsAsync())
                .Select(ac => (PartnerIdentityProviderConfiguration)ac)
                .ToList();
        }
    }
}
