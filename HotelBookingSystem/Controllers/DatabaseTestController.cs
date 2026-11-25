using HotelBookingSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelBookingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DatabaseTestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatabaseTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                _context.RoomTypes.ToList();
                _context.BookingStatuses.ToList();

                return Json(new
                {
                    Message = "Database operations successful."
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "The database connection test failed. Please try again later.");
            }
        }
    }
}