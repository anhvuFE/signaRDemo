using System.ComponentModel.DataAnnotations;

namespace Assignment3.Models
{
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}