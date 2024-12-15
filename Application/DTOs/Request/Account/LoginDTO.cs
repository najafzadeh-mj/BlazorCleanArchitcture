using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Account
{
    public class LoginDTO
    {
        [EmailAddress,Required]
        [Display(Name ="Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
