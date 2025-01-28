using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Data;
using BookingSalonApp.Models;
using System.Linq;
namespace BookingSalonApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Salons
        public IActionResult Salons()
        {
            var salons = _context.Salons.ToList();
            return View(salons);
        }
        // GET: Admin/CreateSalon
        public IActionResult CreateSalon()
        {
            return View();
        }
        // POST: Admin/CreateSalon
        [HttpPost]
        public IActionResult CreateSalon(Salon salon)
        {
            if (ModelState.IsValid)
            {
                _context.Salons.Add(salon);
                _context.SaveChanges();
                return RedirectToAction(nameof(Salons));
            }
            return View(salon);
        }
        // GET: Admin/DeleteSalon/5
        [HttpGet]
        public IActionResult DeleteSalon(int id)
        {
            var salon = _context.Salons.Find(id);
            if (salon == null)
            {
                return NotFound();
            }
            return View(salon);
        }
        // POST: Admin/DeleteSalon/5
        [HttpPost, ActionName("DeleteSalon")]
        public IActionResult DeleteSalonConfirmed(int id)
        {
            var salon = _context.Salons.Find(id);
            if (salon != null)
            {
                _context.Salons.Remove(salon);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Salons));
        }
        // GET: Admin/EditSalon/5
        [HttpGet]
        public IActionResult EditSalon(int id)
        {
            var salon = _context.Salons
                .Include(s => s.Employees) // Make sure employees are included
                .FirstOrDefault(s => s.Id == id);
            if (salon == null)
            {
                return NotFound();
            }
            return View(salon);
        }
        // POST: Admin/EditSalon
        [HttpPost]
        public IActionResult EditSalon(Salon salon, string newEmployeeName)
        {
            // Check if a new employee name was provided and create it
            if (!string.IsNullOrWhiteSpace(newEmployeeName))
            {
                var newEmployee = new Employee
                {
                    Name = newEmployeeName,
                    SalonId = salon.Id // Associate with the current salon
                };
                _context.Employees.Add(newEmployee); // Add to the Employees DbSet
                salon.Employees.Add(newEmployee); // Add to the salon's employees list
            }
            if (ModelState.IsValid)
            {
                _context.Update(salon); // Update the salon details
                _context.SaveChanges(); // Save changes to the database
                return RedirectToAction(nameof(Salons)); // Redirect back to the list of salons
            }
            return View(salon); // Return to view if the model state is invalid
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var salon = _context.Salons
                .Include(s => s.Employees) // Include employees for this salon
                .Include(s => s.Services)   // Include services if necessary
                .FirstOrDefault(s => s.Id == id);

            if (salon == null)
            {
                return NotFound(); // Handle salon not found
            }
            return View(salon); // Pass the salon with its employees to the view
        }

    }
}
