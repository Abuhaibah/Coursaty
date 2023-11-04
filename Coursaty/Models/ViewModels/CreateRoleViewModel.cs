using System.ComponentModel.DataAnnotations;

namespace Coursaty.Models.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string? RoleName { get; set; }
    }
}
