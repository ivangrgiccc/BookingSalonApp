using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Data;
using BookingSalonApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace BookingSalonApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private async Task<IActionResult> CheckAdminRole()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return currentUser == null ? Unauthorized() : Forbid();
            }
            return null;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Salons()
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            var salons = _context.Salons.Include(s => s.Employees).ToList();
            return View(salons);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSalon()
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            var employees = await _context.Employees.ToListAsync();

            ViewBag.Employees = employees;

            return View();
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSalon(Salon salon, IFormFile logoFile, List<int> employeeIds, bool addEmployees)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            if (salon.WorkingHours == null)
            {
                salon.WorkingHours = new List<WorkingHour>();
            }

            if (logoFile != null && logoFile.Length > 0)
            {
                var fileExtension = Path.GetExtension(logoFile.FileName);
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(fileStream);
                }

                salon.ImagePath = "images/logos/" + fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    salon.Employees = new List<Employee>();

                    if (addEmployees && employeeIds != null && employeeIds.Any())
                    {
                        var employees = await _context.Employees.Where(e => employeeIds.Contains(e.Id)).ToListAsync();
                        salon.Employees = employees;
                    }

                    _context.Salons.Add(salon);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Salons));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error saving to database: " + ex.InnerException?.Message);
                }
            }

            ViewBag.Employees = await _context.Employees.ToListAsync();
            return View(salon);
        }



        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSalon(int id)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            var salon = await _context.Salons.FindAsync(id);
            if (salon == null) return NotFound();

            return View(salon);
        }

        [HttpPost, ActionName("DeleteSalon")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSalonConfirmed(int id)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            var salon = await _context.Salons
                .Include(s => s.Reservations)   // Uključi sve rezervacije povezane sa salonom
                .Include(s => s.Employees)      // Uključi sve zaposlenike povezane sa salonom
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salon == null) return NotFound();

            // Briši sve rezervacije povezane s ovim salonu
            foreach (var reservation in salon.Reservations)
            {
                _context.Reservations.Remove(reservation);
            }

            // Briši sve zaposlenike povezane s ovim salonu
            foreach (var employee in salon.Employees)
            {
                _context.Employees.Remove(employee);
            }

            // Briši salon
            _context.Salons.Remove(salon);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error saving to database: " + ex.InnerException?.Message);
                return View(salon); // Ako dođe do greške, vrati prikaz salona
            }

            return RedirectToAction(nameof(Salons));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddService(int salonId, string serviceName, decimal price)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            // Provjera naziva usluge
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                ModelState.AddModelError("ServiceName", "Naziv usluge je obavezno polje.");
            }

            // Provjera cijene
            if (price <= 0)
            {
                ModelState.AddModelError("Price", "Cijena mora biti pozitivna vrijednost.");
            }

            // Provjera za maksimalno dvije decimale
            if (price != Math.Round(price, 2))
            {
                ModelState.AddModelError("Price", "Cijena može imati najviše dvije decimale.");
            }

            // Ako su podaci validni, spremit ćemo uslugu
            if (ModelState.IsValid)
            {
                var salon = await _context.Salons.Include(s => s.Services).FirstOrDefaultAsync(s => s.Id == salonId);
                if (salon == null) return NotFound();

                var newService = new Service
                {
                    Name = serviceName,
                    Price = price,
                    SalonId = salon.Id
                };

                _context.Services.Add(newService);
                salon.Services.Add(newService);

                await _context.SaveChangesAsync();
                return RedirectToAction("Salons");
            }

            // Ako nije validno, vraćamo formu s greškama
            return View();
        }



        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddService(int salonId)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            var salon = await _context.Salons
                .Include(s => s.Services)
                .FirstOrDefaultAsync(s => s.Id == salonId);

            if (salon == null) return NotFound();

            return View(salon);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return RedirectToAction("AddService", new { salonId = service.SalonId });
        }

        //EDITSALON
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSalon(int id)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            var salon = await _context.Salons
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salon == null)
                return NotFound();

            ViewBag.Employees = await _context.Employees.ToListAsync();

            return View(salon);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSalon(Salon salon)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            var existingSalon = await _context.Salons
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(s => s.Id == salon.Id);

            if (existingSalon == null)
                return NotFound();

            existingSalon.Employees.Clear();

            if (salon.Employees != null)
            {
                foreach (var employee in salon.Employees)
                {
                    if (!string.IsNullOrWhiteSpace(employee.Name))
                    {
                        existingSalon.Employees.Add(new Employee
                        {
                            Name = employee.Name,
                            SalonId = existingSalon.Id
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Salons));



        }
    }
}
