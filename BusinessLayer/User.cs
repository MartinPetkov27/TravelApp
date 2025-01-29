using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;

namespace BusinessLayer
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        #region Basic User's properties
        [Required]
        [MaxLength(50, ErrorMessage = "The username should not be longer that 50 symbols.")]
        public string Username { get; set; }
        
        [MinLength(10, ErrorMessage = "The password should be at least 10 symbols long.")]
        [Required]
        public string Password { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email.")]
        [Required]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "The User's first name should not be longer that 50 symbols.")]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The User's last name should not be longer that 50 symbols.")]
        [Required]
        public string LastName { get; set; }

        [Required]
        public RoleType Role { get; set; }
        #endregion

        public ICollection<Trip> Trips { get; set; }
        public ICollection<BucketList> BucketLists { get; set; }
        public ICollection<Story> Stories { get; set; }

        //Constructors
        public User() { }
        public User(string username, string password, string email, string firstName, string lastName, string address, string phoneNumber, RoleType role)
        {
            Username = username;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
