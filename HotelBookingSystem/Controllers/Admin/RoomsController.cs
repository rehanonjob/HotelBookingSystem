using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Include(room => room.RoomType)
                .OrderBy(room => room.RoomNumber)
                .ToListAsync();

            return View("~/Views/Admin/Rooms/Index.cshtml", rooms);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRoomTypes();

            return View("~/Views/Admin/Rooms/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddRoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRoomTypes();
                return View("~/Views/Admin/Rooms/Create.cshtml", model);
            }

            var roomNumberExists = await _context.Rooms
                .AnyAsync(room => room.RoomNumber == model.RoomNumber);

            if (roomNumberExists)
            {
                ModelState.AddModelError(
                    nameof(model.RoomNumber),
                    "A room with this room number already exists.");

                await LoadRoomTypes();
                return View("~/Views/Admin/Rooms/Create.cshtml", model);
            }

            var roomTypeExists = await _context.RoomTypes
                .AnyAsync(roomType => roomType.RoomTypeId == model.RoomTypeId);

            if (!roomTypeExists)
            {
                ModelState.AddModelError(
                    nameof(model.RoomTypeId),
                    "Please select a valid room type.");

                await LoadRoomTypes();
                return View("~/Views/Admin/Rooms/Create.cshtml", model);
            }

            var room = new Room
            {
                RoomNumber = model.RoomNumber,
                RoomTypeId = model.RoomTypeId,
                PricePerNight = model.PricePerNight,
                Description = model.Description,
                IsActive = model.IsActive
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(room => room.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            await LoadRoomTypes();

            var model = new EditRoomViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomTypeId = room.RoomTypeId,
                PricePerNight = room.PricePerNight,
                Description = room.Description,
                IsActive = room.IsActive
            };

            return View("~/Views/Admin/Rooms/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRoomTypes();
                return View("~/Views/Admin/Rooms/Edit.cshtml", model);
            }

            if (id != model.RoomId)
            {
                return BadRequest();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(room => room.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomNumberExists = await _context.Rooms
                .AnyAsync(otherRoom =>
                    otherRoom.RoomNumber == model.RoomNumber &&
                    otherRoom.RoomId != id);

            if (roomNumberExists)
            {
                ModelState.AddModelError(
                    nameof(model.RoomNumber),
                    "A room with this room number already exists.");

                await LoadRoomTypes();
                return View("~/Views/Admin/Rooms/Edit.cshtml", model);
            }

            var roomTypeExists = await _context.RoomTypes
                .AnyAsync(roomType => roomType.RoomTypeId == model.RoomTypeId);

            if (!roomTypeExists)
            {
                ModelState.AddModelError(
                    nameof(model.RoomTypeId),
                    "Please select a valid room type.");

                await LoadRoomTypes();
                return View("~/Views/Admin/Rooms/Edit.cshtml", model);
            }

            room.RoomNumber = model.RoomNumber;
            room.RoomTypeId = model.RoomTypeId;
            room.PricePerNight = model.PricePerNight;
            room.Description = model.Description;
            room.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(room => room.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            room.IsActive = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(room => room.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            var hasUpcomingConfirmedBooking = await _context.Bookings
                .AnyAsync(booking =>
                    booking.RoomId == id &&
                    booking.BookingStatus.StatusName == "Confirmed" &&
                    booking.CheckInDate >= DateTime.Today);

            if (hasUpcomingConfirmedBooking)
            {
                TempData["ErrorMessage"] =
                    "This room cannot be deactivated because it has an upcoming confirmed booking.";

                return RedirectToAction(nameof(Index));
            }

            room.IsActive = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRoomTypes()
        {
            ViewBag.RoomTypes = await _context.RoomTypes
                .OrderBy(roomType => roomType.TypeName)
                .ToListAsync();
        }
    }
}
