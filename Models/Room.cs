using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalityProject.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        [Required]
        public string RoomType { get; set; } // e.g., "Standard", "Deluxe", "Suite"
        
        [Required]
        public int Capacity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PricePerNight { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

        [Required]
        public string RoomDescription { get; set; }
    }
}
