using System.ComponentModel.DataAnnotations;

namespace Coursaty.Models.ViewModels
{
	public class CourseViewModel
	{
		[Key]
		public int CourseId { get; set; }
		public string CourseName { get; set; }
		public string CourseDescription { get; set; }
		public DateTime CourseStartDate { get; set; }
		public string? CourseImg { get; set; }
		public int CourseHours { get; set; }
        public string InstructorName { get; set; }
       

    }
}
