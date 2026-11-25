using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models.ViewModels
{
    public class EditRoomViewModel
    {
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(20, ErrorMessage = "Room number cannot be longer than 20 characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room type is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid room type.")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Price per night is required.")]
        [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "Price per night must be greater than zero and cannot exceed 99,999,999.99.")]
        public decimal PricePerNight { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
