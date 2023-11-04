using System.Collections.Generic;
using Coursaty.Models.ViewModels;

namespace Coursaty.Models.ViewModels
{
    public class ChooseStudentsViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<AddUserViewModel> AvailableStudents { get; set; }
    }
}
