using HotelBookingSystem.Data;
using HotelBookingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(booking => booking.User)
                .Include(booking => booking.Room)
                .ThenInclude(room => room.RoomType)
                .Include(booking => booking.BookingStatus)
                .OrderByDescending(booking => booking.BookingDate)
                .ToListAsync();

            var model = bookings
                .Select(booking => new AdminBookingListViewModel
                {
                    BookingId = booking.BookingId,
                    GuestName = booking.User.FullName,
                    GuestEmail = booking.User.Email,
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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(booking => booking.User)
                .Include(booking => booking.Room)
                .ThenInclude(room => room.RoomType)
                .Include(booking => booking.BookingStatus)
                .FirstOrDefaultAsync(booking => booking.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            var model = new AdminBookingDetailsViewModel
            {
                BookingId = booking.BookingId,
                GuestName = booking.User.FullName,
                GuestEmail = booking.User.Email,
                GuestPhone = booking.User.PhoneNumber,
                RoomNumber = booking.Room.RoomNumber,
                RoomType = booking.Room.RoomType.TypeName,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalNights = (booking.CheckOutDate - booking.CheckInDate).Days,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus.StatusName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var booking = await _context.Bookings
                .Include(booking => booking.BookingStatus)
                .FirstOrDefaultAsync(booking => booking.BookingId == id);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "The booking could not be found.";
                return RedirectToAction(nameof(Index));
            }

            if (booking.BookingStatus.StatusName != "Pending")
            {
                TempData["ErrorMessage"] =
                    "Only Pending bookings can be confirmed.";

                return RedirectToAction(nameof(Details), new { id });
            }

            var confirmedStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(status => status.StatusName == "Confirmed");

            if (confirmedStatus == null)
            {
                TempData["ErrorMessage"] =
                    "The Confirmed booking status is not available.";

                return RedirectToAction(nameof(Details), new { id });
            }

            booking.BookingStatusId = confirmedStatus.BookingStatusId;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "The booking was confirmed successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .Include(booking => booking.BookingStatus)
                .FirstOrDefaultAsync(booking => booking.BookingId == id);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "The booking could not be found.";
                return RedirectToAction(nameof(Index));
            }

            if (booking.BookingStatus.StatusName != "Pending" &&
                booking.BookingStatus.StatusName != "Confirmed")
            {
                TempData["ErrorMessage"] =
                    "Only Pending or Confirmed bookings can be cancelled.";

                return RedirectToAction(nameof(Details), new { id });
            }

            var cancelledStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(status => status.StatusName == "Cancelled");

            if (cancelledStatus == null)
            {
                TempData["ErrorMessage"] =
                    "The Cancelled booking status is not available.";

                return RedirectToAction(nameof(Details), new { id });
            }

            booking.BookingStatusId = cancelledStatus.BookingStatusId;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "The booking was cancelled successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
