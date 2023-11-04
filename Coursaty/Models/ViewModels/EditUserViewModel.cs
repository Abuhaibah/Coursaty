using System.ComponentModel.DataAnnotations;

namespace Coursaty.Models.ViewModels
{
    public class EditUserViewModel
    {
        public int userId { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [MaxLength(40, ErrorMessage = "Max 40 characters")]
        [EmailAddress(ErrorMessage = "Example: User@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter mobile")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }

    }

}
