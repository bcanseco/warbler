using System.Collections.Generic;
using IdentityServer4.Models;

namespace Warbler.Misc
{
    public static class IdentityServerConfig
    {
        /// <summary>
        ///   Define the scopes
        /// </summary>
        public static IEnumerable<ApiResource> GetApiResource()
            => new List<ApiResource>
            {
                new ApiResource("api1", "my api")
            };

        /// <summary>
        ///   Register in the Client
        /// </summary>
        public static IEnumerable<Client> GetClient()
            => new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("secrect".Sha256())},
                    AllowedScopes = {"api"}
                }
            };
    }
}
