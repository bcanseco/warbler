using System.ComponentModel.DataAnnotations;

namespace Warbler.Models.ManageViewModels
{
    public class ClaimUniversityViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name", Prompt = "Enter your first name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name", Prompt = "Enter your last name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address", Prompt = "Enter your email address")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Position", Prompt = "Enter your position at the claimed university")]
        public string PositionTitle { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name of University", Prompt = "Enter the name of the claimed university")]
        public string UniversityName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "University Website", Prompt = "Enter the claimed university's website")]
        public string UniversityWebsite { get; set; }
    }
}
