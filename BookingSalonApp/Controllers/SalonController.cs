using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Data;
using BookingSalonApp.Models;

namespace BookingSalonApp.Controllers
{
    public class SalonController : Controller
    {
        private readonly AppDbContext _context;
        public SalonController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var salons = _context.Salons.ToList();

            return View(salons);
        }
        public IActionResult Details(int id)
        {
            var salon = _context.Salons.Where(s => s.Id == id).FirstOrDefault();
            
            if (salon == null)
                return NotFound();
            salon.Employees = _context.Employees.Where(e => e.SalonId == id).ToList();
            salon.Services = _context.Services.Where(s => s.Id == id).ToList();
            return View(salon);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Salon salon)
        {
            if (ModelState.IsValid)
            {
                _context.Salons.Add(salon);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salon);
        }
        public IActionResult Book(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reservation = new Reservation
                {
                    UserId =model.UserId,
                    SalonId = model.SalonId,
                    EmployeeId = model.EmployeeId,
                    Date = model.Date,
                };
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Book(int salonId)
        {
            var salon = _context.Salons
                .Include(s => s.Employees) 
                .Include(s => s.Services) 
                .FirstOrDefault(s => s.Id == salonId);
            if (salon == null)
            {
                return NotFound(); 
            }
            var bookingViewModel = new BookingViewModel
            {
                SalonId = salon.Id,
                SalonName = salon.Name,
                Employees = salon.Employees, 
                                            
            };
            return View("~/Views/Reservation/Book.cshtml", bookingViewModel);
        }
    }
}
