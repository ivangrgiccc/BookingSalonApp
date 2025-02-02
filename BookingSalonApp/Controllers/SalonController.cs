using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Data;
using BookingSalonApp.Models;
using System.Linq;
using System.Collections.Generic;

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
            var salon = _context.Salons
                .Include(s => s.Employees)
                .Include(s => s.Services)
                .FirstOrDefault(s => s.Id == id);

            if (salon == null)
                return NotFound();

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

        public IActionResult Edit(int id)
        {
            var salon = _context.Salons
                .Include(s => s.Employees)
                .Include(s => s.Services)
                .FirstOrDefault(s => s.Id == id);

            if (salon == null)
                return NotFound();

            return View(salon);
        }

        [HttpPost]
        public IActionResult Edit(int id, Salon salon, List<Service> Services, string DeletedEmployeeIds, string DeletedServiceIds)
        {
            if (ModelState.IsValid)
            {
                var existingSalon = _context.Salons
                    .Include(s => s.Employees)
                    .Include(s => s.Services)
                    .FirstOrDefault(s => s.Id == id);

                if (existingSalon == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(DeletedEmployeeIds))
                {
                    var deletedEmployeeIds = DeletedEmployeeIds.Split(',').Select(int.Parse).ToList();
                    var deletedEmployees = _context.Employees.Where(e => deletedEmployeeIds.Contains(e.Id)).ToList();
                    _context.Employees.RemoveRange(deletedEmployees);
                }

                if (!string.IsNullOrEmpty(DeletedServiceIds))
                {
                    var deletedServiceIds = DeletedServiceIds.Split(',').Select(int.Parse).ToList();
                    var deletedServices = _context.Services.Where(s => deletedServiceIds.Contains(s.Id)).ToList();
                    _context.Services.RemoveRange(deletedServices);
                }

                foreach (var service in Services)
                {
                    service.SalonId = salon.Id;
                    _context.Services.Add(service);
                }

                existingSalon.Name = salon.Name;
                existingSalon.Location = salon.Location;
                existingSalon.Address = salon.Address;
                existingSalon.WorkingHours = salon.WorkingHours;
                existingSalon.GoogleMapsIframe = salon.GoogleMapsIframe;
                existingSalon.ImagePath = salon.ImagePath;

                _context.SaveChanges();

                return RedirectToAction("Details", new { id = salon.Id });
            }

            return View(salon);
        }
    }
}
