using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BucketList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The name of the BucketList should not be longer that 50 symbols.")]
        public string Name { get; set; }

        public ICollection<Place>? Destinations { get; set; }
        public ICollection<Country>? Countries { get; set; }

        public string? Place1 { get; set; }
        public string? ImageUrl1 { get; set; }
        public string? Place2 { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? Place3 { get; set; }
        public string? ImageUrl3{ get; set; }
        public string? Place4 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? Place5 { get; set; }
        public string? ImageUrl5 { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }

        public BucketList() { }
        public BucketList( ICollection<Place> destination, ICollection<Country> countries, User user)
        {
            Destinations = destination;
            Countries = countries;
            User = user;
            UserId = user.Id;
        }

    }
}
