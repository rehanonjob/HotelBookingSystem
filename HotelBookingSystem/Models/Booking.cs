namespace HotelBookingSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public DateTime BookingDate { get; set; }

        public int GuestCount { get; set; }

        public decimal TotalAmount { get; set; }

        public int BookingStatusId { get; set; }

        public string? SpecialRequests { get; set; }

        public User User { get; set; } = null!;

        public Room Room { get; set; } = null!;

        public BookingStatus BookingStatus { get; set; } = null!;
    }
}