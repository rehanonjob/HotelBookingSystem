namespace HotelBookingSystem.Models
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MaxOccupancy { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
