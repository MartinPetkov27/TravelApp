using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Place
    {
        //GeoNames
        //there are three-symboled identification code
        //Eeach place has an unique code(it consists of 7/8 characters)

        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        ///[Required]
        public double Latitude { get; set; }

        //[Required]
        public double Longitude { get; set; }

        [ForeignKey("Country")]
        public string CoutryAlphaCode { get; set; }
        [Required]
        public Country Country { get; set; }

        [ForeignKey("Trip")]
        public int TripId { get; set; }
        //[Required]
        public Trip Trip { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public Place() { }

        public Place(string name, double latitude, double longitude, Country country)
        { 
            Name = name; 
            Latitude = latitude; 
            Longitude = longitude; 
            Country = country;
            CoutryAlphaCode = country.AlphaCode;
        }
        public Place(string name, Country country, string imageUrl)
        {
            Name = name;
            Country = country;
            CoutryAlphaCode = country.AlphaCode;
            ImageUrl = imageUrl;
        }
    }
}
