using System.ComponentModel.DataAnnotations;

namespace Assignment3.Models
{
    public class PostCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string Description { get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}