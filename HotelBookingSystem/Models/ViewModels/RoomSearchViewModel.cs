using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Models.ViewModels
{
    public class RoomSearchViewModel : IValidatableObject
    {
        [DataType(DataType.Date)]
        public DateTime? CheckInDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CheckOutDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid room type.")]
        public int? RoomTypeId { get; set; }

        public List<RoomType> AvailableRoomTypes { get; set; } = new List<RoomType>();

        public List<Room> AvailableRooms { get; set; } = new List<Room>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckInDate.HasValue &&
                CheckOutDate.HasValue &&
                CheckOutDate.Value <= CheckInDate.Value)
            {
                yield return new ValidationResult(
                    "Check-out date must be later than check-in date.",
                    new[] { nameof(CheckOutDate) });
            }
        }
    }
}
