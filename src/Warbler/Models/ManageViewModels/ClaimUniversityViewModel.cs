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
        
        //[Required]
        [DataType(DataType.Text)]
        [Display(Name = "University", Prompt = "Which university are you claiming?")]
        public string UniversityName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Position", Prompt = "Enter your position title at this university")]
        public string PositionTitle { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Comments", Prompt = "Any other comments or supporting documentation?")]
        public string Comments { get; set; }
    }
}
