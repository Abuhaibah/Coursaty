using System.ComponentModel.DataAnnotations;

namespace Coursaty.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        public string? RoleId { get; set; }
        [Required(ErrorMessage = "Enter Role Name")]
        [MinLength(3, ErrorMessage = "Min 3 Char")]
        [MaxLength(25, ErrorMessage = "Max 25 Char")]
        public string? RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
