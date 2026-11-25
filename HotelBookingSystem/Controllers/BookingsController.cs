using HotelBookingSystem.Data;
using HotelBookingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    [Authorize(Roles = "Guest")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int roomId)
        {
            var room = await _context.Rooms
                .Include(room => room.RoomType)
                .FirstOrDefaultAsync(room => room.RoomId == roomId && room.IsActive);

            if (room == null)
            {
                return NotFound();
            }

            var model = new BookingViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomTypeName = room.RoomType.TypeName,
                PricePerNight = room.PricePerNight,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            };

            model.TotalNights = (model.CheckOutDate - model.CheckInDate).Days;
            model.TotalAmount = model.PricePerNight * model.TotalNights;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            var room = await _context.Rooms
                .Include(room => room.RoomType)
                .FirstOrDefaultAsync(room => room.RoomId == model.RoomId);

            if (room == null)
            {
                return NotFound();
            }

            model.RoomNumber = room.RoomNumber;
            model.RoomTypeName = room.RoomType.TypeName;
            model.PricePerNight = room.PricePerNight;

            if (!room.IsActive)
            {
                ModelState.AddModelError(string.Empty, "This room is no longer available for booking.");
                return View(model);
            }

            if (model.GuestCount < 1)
            {
                ModelState.AddModelError(
                    nameof(model.GuestCount),
                    "At least one guest is required.");
            }
            else if (model.GuestCount > room.RoomType.MaxOccupancy)
            {
                ModelState.AddModelError(
                    nameof(model.GuestCount),
                    $"The guest count cannot exceed this room type's maximum occupancy of {room.RoomType.MaxOccupancy}.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.CheckInDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(model.CheckInDate),
                    "Check-in date must be today or a future date.");
            }

            if (model.CheckOutDate <= model.CheckInDate)
            {
                ModelState.AddModelError(
                    nameof(model.CheckOutDate),
                    "Check-out date must be later than check-in date.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int totalNights = (model.CheckOutDate - model.CheckInDate).Days;
            decimal totalAmount = room.PricePerNight * totalNights;

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                return Unauthorized();
            }

            var pendingStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(status => status.StatusName == "Pending");

            if (pendingStatus == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The Pending booking status is not available.");

                return View(model);
            }

            var booking = new HotelBookingSystem.Models.Booking
            {
                UserId = userId,
                RoomId = room.RoomId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                BookingDate = DateTime.UtcNow,
                GuestCount = model.GuestCount,
                TotalAmount = totalAmount,
                BookingStatusId = pendingStatus.BookingStatusId
            };

            await using var transaction = await _context.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                var hasOverlappingBooking = await _context.Bookings
                    .AnyAsync(existingBooking =>
                        existingBooking.RoomId == model.RoomId &&
                        (existingBooking.BookingStatus.StatusName == "Pending" ||
                         existingBooking.BookingStatus.StatusName == "Confirmed") &&
                        existingBooking.CheckInDate < model.CheckOutDate &&
                        existingBooking.CheckOutDate > model.CheckInDate);

                if (hasOverlappingBooking)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "This room is already booked for the selected dates.");

                    await transaction.RollbackAsync();
                    return View(model);
                }

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return RedirectToAction("Confirmation", new { id = booking.BookingId });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                return Unauthorized();
            }

            var booking = await _context.Bookings
                .Include(booking => booking.Room)
                .ThenInclude(room => room.RoomType)
                .Include(booking => booking.BookingStatus)
                .FirstOrDefaultAsync(booking =>
                    booking.BookingId == id &&
                    booking.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                return Unauthorized();
            }

            var bookings = await _context.Bookings
                .Where(booking => booking.UserId == userId)
                .Include(booking => booking.Room)
                .ThenInclude(room => room.RoomType)
                .Include(booking => booking.BookingStatus)
                .OrderByDescending(booking => booking.BookingDate)
                .ToListAsync();

            var model = bookings
                .Select(booking => new MyBookingViewModel
                {
                    BookingId = booking.BookingId,
                    RoomNumber = booking.Room.RoomNumber,
                    RoomType = booking.Room.RoomType.TypeName,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    TotalNights = (booking.CheckOutDate - booking.CheckInDate).Days,
                    TotalAmount = booking.TotalAmount,
                    BookingStatus = booking.BookingStatus.StatusName
                })
                .ToList();

            return View(model);
        }
    }
}
