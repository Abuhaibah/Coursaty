using Coursaty.Models;
using Coursaty.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coursaty.Data
{
	public class AppDbContext : IdentityDbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}
		public DbSet<CourseViewModel> Courses { get; set; }
		public DbSet<AddUserViewModel> AddUser { get; set; }
		public DbSet<CarouselItem> CarouselItems { get; set; }
		public DbSet<MenuItem> MenuItems { get; set; }



    }
}
