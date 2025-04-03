using BusinessLayer;

namespace PresentationLayer.ViewModels
{
    public class EditUserViewModel
    {
        public User User { get; set; }
        public RoleType Role { get; set; } // Store a single role
    }
}
