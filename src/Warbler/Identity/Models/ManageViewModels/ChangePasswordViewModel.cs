using System.ComponentModel.DataAnnotations;

namespace Warbler.Identity.Models.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password", Prompt = "Enter your current password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password", Prompt = "Pick a new password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password", Prompt = "Enter the new password again")]
        [Compare("NewPassword", ErrorMessage = "Your passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
