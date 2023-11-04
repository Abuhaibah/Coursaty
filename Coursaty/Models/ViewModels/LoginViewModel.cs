using System.ComponentModel.DataAnnotations;

namespace Coursaty.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter email")]
        [MaxLength(40, ErrorMessage = "Max 40 char")]
        [EmailAddress(ErrorMessage = "Example:User@gmail.com")]

        public string? Email { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
