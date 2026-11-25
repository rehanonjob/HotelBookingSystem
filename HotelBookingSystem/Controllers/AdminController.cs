using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalRooms = await _context.Rooms.CountAsync(),
                ActiveRooms = await _context.Rooms.CountAsync(room => room.IsActive),
                TotalBookings = await _context.Bookings.CountAsync(),
                PendingBookings = await _context.Bookings
                    .CountAsync(booking => booking.BookingStatus.StatusName == "Pending"),
                ConfirmedBookings = await _context.Bookings
                    .CountAsync(booking => booking.BookingStatus.StatusName == "Confirmed"),
                CancelledBookings = await _context.Bookings
                    .CountAsync(booking => booking.BookingStatus.StatusName == "Cancelled")
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View("~/Areas/Admin/Views/Admin/CreateAdmin.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(CreateAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Areas/Admin/Views/Admin/CreateAdmin.cshtml", model);
            }

            var emailExists = await _context.Users
                .AnyAsync(user => user.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View("~/Areas/Admin/Views/Admin/CreateAdmin.cshtml", model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "The admin account was created successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
