using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;

namespace BusinessLayer
{
    public class Trip
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; } 

        public DateTime? StartingDate { get; set; }

        public DateTime? EndingDate { get; set; }

        public string? FirstCountryAlphaCode { get; set; }

        public ICollection<Country>? Countries { get; set; }

        ////starting place
        //[ForeignKey("StaritingPlace")]
        //public int StartingPlaceId { get; set; }
        [Required]
        [NotMapped]
        public Place StartingPlace { get; set; }

        ////ending place
        //[ForeignKey("EndingPlace")]
        //public int EndingPlaceId { get; set; }
        [Required]
        [NotMapped]
        public Place EndingPlace { get; set; }

        //places
        public ICollection<Place> Places { get; set; }

        //relation with user -> many-to-one from the perspective of the trip
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User{ get; set; }

        //constructors
        public Trip() { }
        public Trip( string title, string firstCountryAlphaCode, List<Country> countries, ICollection<Place> places, int userId, User user)
        {
            Title = title;
            FirstCountryAlphaCode = firstCountryAlphaCode;
            Countries = countries;
            Places = places;
            StartingPlace = places.First();
            EndingPlace = places.Last();
            User = user;
            UserId = user.Id;
        }
        public Trip(string title, string firstCountryAlphaCode, ICollection<Place> places, int userId, User user)
        {
            Title = title;
            FirstCountryAlphaCode = firstCountryAlphaCode;
            Countries = new List<Country>();
            Places = places;
            StartingPlace = places.First();
            EndingPlace = places.Last();
            User = user;
            UserId = user.Id;
        }
    }
}
