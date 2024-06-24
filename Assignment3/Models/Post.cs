using System.ComponentModel.DataAnnotations;

namespace Assignment3.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public int AuthorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int PublishStatus { get; set; }
        public int CategoryId { get; set; }

        public AppUser? Author { get; set; }
        public PostCategory? Category { get; set; }
    }
}