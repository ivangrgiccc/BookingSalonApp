using Microsoft.AspNetCore.Mvc;
using BookingSalonApp.Data;
using BookingSalonApp.Models;

namespace BookingSalonApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        public ReservationController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult MyReservations(int userId)
        {
            var reservations = _context.Reservations.Where(r => r.UserId == userId).ToList();
            return View(reservations);
        }
        public IActionResult Book(int salonId)
        {
            var salon = _context.Salons.Find(salonId);
            if (salon == null)
                return NotFound();
            ViewBag.Services = _context.Services.Where(s => s.Id == salonId).ToList();
            ViewBag.Employees = _context.Employees.Where(e => e.SalonId == salonId).ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Book(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return RedirectToAction("MyReservations", new { userId = reservation.UserId });
            }
            return View(reservation);
        }
    }
}
