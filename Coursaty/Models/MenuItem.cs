using System.ComponentModel.DataAnnotations;

namespace Coursaty.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuId { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public List<MenuItem> SubMenu { get; set; }
    }

}
