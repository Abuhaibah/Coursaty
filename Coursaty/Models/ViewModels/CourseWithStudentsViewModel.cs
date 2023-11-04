using System.Collections.Generic;

namespace Coursaty.Models.ViewModels
{
    public class CourseWithStudentsViewModel
    {
        public CourseViewModel Course { get; set; }
        public List<AddUserViewModel> Students { get; set; }
    }

}
