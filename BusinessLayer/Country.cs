using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer
{
    public class Country
    {
        [Key]
        [Required]
        public string AlphaCode { get; set; } // it consists of three letters 

        [Required]
        [MaxLength(100, ErrorMessage = "The country name should not be longer than 70 synmols.")]
        public string Name { get; set; }

        public string Capital { get; set; }
        
        public string Currency { get; set; }
        
        public string Language { get; set; }

        public ICollection<Trip> Trips{ get; set; }

        //national Flag

        public Country() { }
        public Country(string alphaCode, string name)
        { 
            AlphaCode = alphaCode;
            Name = name;
        }
    }
}
