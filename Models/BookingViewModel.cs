using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalityProject.Models
{
    public class BookingViewModel
    {
        // These properties are for UI population only.
        // [ValidateNever] is added to tell the model binder to ignore them during validation.
        [ValidateNever]
        public IEnumerable<SelectListItem> Rooms { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> BedTypes { get; set; }

        // These properties hold the user's selections and should be validated.
        public int? RoomId { get; set; }
        public string RoomType { get; set; }
        public string BedType { get; set; }


        [Required]
        [StringLength(100)]
        public string GuestName { get; set; }

        [Required]
        [StringLength(100)]
        public string GuestLastName { get; set; }


        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }


        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }


        [Range(1, 4, ErrorMessage = "Number of guests must be at least 1.")]
        public int NumberOfGuests { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateTime? CheckInDate { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateTime? CheckOutDate { get; set; }

        // These are calculated on the server, not submitted by the user.

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }
    }
}