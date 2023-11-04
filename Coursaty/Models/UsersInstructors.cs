using Coursaty.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Coursaty.Models
{
    public class UsersInstructors

    {
        public UsersInstructors()
        {
            CarouselItems = new List<CarouselItem>();
        }
        public IList<CourseViewModel> Courses { get; set; }
        public IList<AddUserViewModel> AddUsers { get; set; }
        public IList<LoginViewModel> Login { get; set; }
        public IList<CourseViewModel> AvailableCourses { get; set; }
        public IList<CarouselItem> CarouselItems { get; set; }

    public IList<MenuItem> MenuItems { get; set; }
        public IEnumerable<IdentityRole> IdentityRoles { get; set; }

        public IList<EditRoleViewModel> EditRoleViewModels { get; set; }
        public IList<UserRoleViewModel> UserRoleViewModels { get; set; }
    }
}
