
using HotelBookingSystem.Data;
using HotelBookingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(RoomSearchViewModel model)
        {
            model.AvailableRoomTypes = await _context.RoomTypes
                .OrderBy(roomType => roomType.TypeName)
                .ToListAsync();

            var roomsQuery = _context.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.IsActive);

            if (model.RoomTypeId.HasValue)
            {
                roomsQuery = roomsQuery
                    .Where(room => room.RoomTypeId == model.RoomTypeId.Value);
            }

            if (model.CheckInDate.HasValue != model.CheckOutDate.HasValue)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please provide both check-in and check-out dates.");
            }
            else if (model.CheckInDate.HasValue && model.CheckOutDate.HasValue)
            {
                var requestedCheckIn = model.CheckInDate.Value;
                var requestedCheckOut = model.CheckOutDate.Value;

                if (requestedCheckIn.Date < DateTime.Today)
                {
                    ModelState.AddModelError(
                        nameof(model.CheckInDate),
                        "Check-in date must be today or a future date.");
                }

                if (requestedCheckOut <= requestedCheckIn)
                {
                    ModelState.AddModelError(
                        nameof(model.CheckOutDate),
                        "Check-out date must be later than check-in date.");
                }

                if (ModelState.IsValid)
                {
                    roomsQuery = roomsQuery
                        .Where(room => !_context.Bookings.Any(existingBooking =>
                            existingBooking.RoomId == room.RoomId &&
                            (existingBooking.BookingStatus.StatusName == "Pending" ||
                             existingBooking.BookingStatus.StatusName == "Confirmed") &&
                            existingBooking.CheckInDate < requestedCheckOut &&
                            existingBooking.CheckOutDate > requestedCheckIn));
                }
            }

            model.AvailableRooms = await roomsQuery
                .OrderBy(room => room.RoomNumber)
                .ToListAsync();

            return View(model);
        }



        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == id && r.IsActive);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


    }
}
