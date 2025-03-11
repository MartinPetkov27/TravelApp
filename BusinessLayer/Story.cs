using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Story
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; } 

        public Status Status { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }

        public Story() { }
        public Story(string title, string content,  int userId, User user)
        { 
            Title = title;
            Content = content;
            User = user;
            UserId = user.Id;
        }
    }
}
