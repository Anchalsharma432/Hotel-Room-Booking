using System.ComponentModel.DataAnnotations;

namespace HospitalityProject.Models
{
    public class Guest
    {
        public int GuestId { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = "";
    }
}
