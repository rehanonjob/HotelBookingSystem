namespace HotelBookingSystem.Models.ViewModels
{
    public class AdminBookingListViewModel
    {
        public int BookingId { get; set; }

        public string GuestName { get; set; } = string.Empty;

        public string GuestEmail { get; set; } = string.Empty;

        public string RoomNumber { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int TotalNights { get; set; }

        public decimal TotalAmount { get; set; }

        public string BookingStatus { get; set; } = string.Empty;
    }
}
