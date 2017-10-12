using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warbler
{
    public class Config
    {
        //Define the scopes
        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","my api")
            };

        }

        //Register in the Client
        public static IEnumerable<Client> GetClient()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="client",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    ClientSecrets={new Secret("secrect".Sha256())},
                    AllowedScopes={"api"}
                }
            };
        }
    }
}