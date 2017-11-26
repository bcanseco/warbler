using System.ComponentModel.DataAnnotations;
using ComponentSpace.Saml2.Configuration;

namespace Warbler.Models
{
    public class AuthConfig
    {
        public int Id { get; set; }
        public int UniversityId { get; set; }
        public University University { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Prompt = "Enter your university website's base URL")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Prompt = "Enter your university's name")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Single-Signon Service URL", Prompt = "Enter your SSO URL")]
        public string SingleSignOnServiceUrl { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Single-Logout Service URL", Prompt = "Enter your SLO URL")]
        public string SingleLogoutServiceUrl { get; set; }

        public static explicit operator PartnerIdentityProviderConfiguration(AuthConfig config)
            => new PartnerIdentityProviderConfiguration
            {
                Name = config.Name,
                Description = config.Description,
                SingleSignOnServiceUrl = config.SingleSignOnServiceUrl,
                SingleLogoutServiceUrl = config.SingleLogoutServiceUrl,
                WantAssertionSigned = false
            };
    }
}
