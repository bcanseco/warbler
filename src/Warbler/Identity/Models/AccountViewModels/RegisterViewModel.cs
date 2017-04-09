using System.ComponentModel.DataAnnotations;

namespace Warbler.Identity.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Enter an email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username", Prompt = "Pick a username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Enter your password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password", Prompt = "Enter your password again")]
        [Compare("Password", ErrorMessage = "Your passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
