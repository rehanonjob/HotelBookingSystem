using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models.ViewModels
{
    public class BookingViewModel : IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid room.")]
        public int RoomId { get; set; }

        [StringLength(20, ErrorMessage = "Room number cannot be longer than 20 characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Room type name cannot be longer than 100 characters.")]
        public string RoomTypeName { get; set; } = string.Empty;

        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Check-in date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Required(ErrorMessage = "At least one guest is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "At least one guest is required.")]
        public int GuestCount { get; set; } = 1;

        public int TotalNights { get; set; }

        public decimal TotalAmount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckInDate == default)
            {
                yield return new ValidationResult(
                    "Check-in date is required.",
                    new[] { nameof(CheckInDate) });
            }

            if (CheckOutDate == default)
            {
                yield return new ValidationResult(
                    "Check-out date is required.",
                    new[] { nameof(CheckOutDate) });
            }

            if (CheckInDate != default &&
                CheckOutDate != default &&
                CheckOutDate <= CheckInDate)
            {
                yield return new ValidationResult(
                    "Check-out date must be later than check-in date.",
                    new[] { nameof(CheckOutDate) });
            }
        }
    }
}
