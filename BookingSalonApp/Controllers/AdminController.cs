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
        public IActionResult Salons()
        {
            var salons = _context.Salons.ToList();
            return View(salons);
        }
        public IActionResult CreateSalon()
        {
            return View();
        }
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

    }
}
