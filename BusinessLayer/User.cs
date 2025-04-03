using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Identity.Core;
using System.Data;
using System.Net;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLayer
{
    public class User : IdentityUser
    {
        //[Key]
        //public int Id { get; set; }

        #region Basic User's properties
        //[Required]
        //[MaxLength(50, ErrorMessage = "The username should not be longer that 50 symbols.")]
        //public string Username { get; set; }
        
        //[MinLength(10, ErrorMessage = "The password should be at least 10 symbols long.")]
        //[Required]
        //public string Password { get; set; }

        //[EmailAddress(ErrorMessage = "Invalid email.")]
        //[Required]
        //public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "The User's first name should not be longer that 50 symbols.")]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The User's last name should not be longer that 50 symbols.")]
        [Required]
        public string LastName { get; set; }

        [MaxLength(15, ErrorMessage = "The User' phone number should not be longer that 15   symbols.")]
        [Required]
        public string PhoneNumber { get; set; }

        //[Required]
        //public RoleType Role { get; set; }
        #endregion

        [NotMapped]
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        [NotMapped]
        public ICollection<BucketList> BucketLists { get; set; } = new List<BucketList>();
        [NotMapped]
        public ICollection<Story> Stories { get; set; } = new List<Story>();

        //Constructors
        public User() { }
        public User(string firstName, string lastName)
        {
            Id = Guid.NewGuid().ToString();
            FirstName = firstName;
            LastName = lastName;
        }
        public User(string userName, string email, string firstName, string lastName, string phoneNumber, RoleType role)
        {
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }
    }
}
