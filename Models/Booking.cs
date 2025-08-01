using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalityProject.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [StringLength(100)]
        public string GuestName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public  string Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [ForeignKey("Room")]
        public int RoomId { get; set; }

        public Room? Room { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be at least 1.")]
        public int NumberOfGuests { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Confirmed";
    }
}
