namespace HospitalityProject.Models
{
    public class BookingSuccessViewModel
    {
        public int BookingId { get; set; }
        public string GuestName { get; set; }
        public string GuestLastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public string BedType { get; set; }
        public int NumberOfGuests { get; set; } = 1;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}
