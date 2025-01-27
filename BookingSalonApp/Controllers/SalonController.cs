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
    }
}
