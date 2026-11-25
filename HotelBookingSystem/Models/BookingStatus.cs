namespace HotelBookingSystem.Models
{
    public class BookingStatus
    {
        public int BookingStatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
