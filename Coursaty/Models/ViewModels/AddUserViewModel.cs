using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Coursaty.Models.ViewModels
{
    public class AddUserViewModel
    {
        [Key]
        public int userId { get; set; }
        [Required(ErrorMessage = "Please enter email")]
        [MaxLength(40, ErrorMessage = "Max 40 char")]
        [EmailAddress(ErrorMessage = "Example:User@gmail.com")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Miss Match")]
        public string? ConfirmPassword { get; set; }
        public string? Mobile { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
		[ForeignKey("CourseViewModel")]
        public int? CourseId { get; set; }

        public CourseViewModel? Course { get; set; }

        public string? Role { get; set; }


    }
}
