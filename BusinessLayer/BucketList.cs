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

        public ICollection<Place> Destinations { get; set; }
        public ICollection<Country> Countries { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }

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
