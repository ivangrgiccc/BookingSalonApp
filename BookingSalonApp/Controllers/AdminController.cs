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

            // Provjeravajmo postoje li zaposlenici u bazi
            var employees = await _context.Employees.ToListAsync();

            // Provodi zaposlenike u View putem ViewBag
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

            // Ako je poslana slika logotipa
            if (logoFile != null && logoFile.Length > 0)
            {
                // Generiramo ime datoteke sa proširenjem
                var fileExtension = Path.GetExtension(logoFile.FileName);
                var fileName = Guid.NewGuid().ToString() + fileExtension;

                // Putanja za pohranu slike u wwwroot/images/logos/
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", fileName);

                // Osiguranje da direktorij postoji
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Pohrana slike na server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(fileStream);
                }

                // Spremanje relativne putanje u bazu podataka
                salon.ImagePath = "images/logos/" + fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ako je označeno, poveži zaposlenike s salonima
                    if (addEmployees && employeeIds != null && employeeIds.Any())
                    {
                        var employees = await _context.Employees.Where(e => employeeIds.Contains(e.Id)).ToListAsync();
                        salon.Employees = employees;
                    }
                    else
                    {
                        // Ako nije označeno, dodaj default zaposlenika (ako ih ima)
                        var defaultEmployee = await _context.Employees.FirstOrDefaultAsync();
                        if (defaultEmployee != null)
                        {
                            salon.Employees = new List<Employee> { defaultEmployee };
                        }
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

            // U slučaju greške, ponovo popuni listu zaposlenika
            var employeesList = await _context.Employees.ToListAsync();
            ViewBag.Employees = employeesList;
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

            var salon = await _context.Salons.FindAsync(id);
            if (salon != null)
            {
                _context.Salons.Remove(salon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Salons));
        }

        //[HttpGet]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> EditSalon(int id)
        //{
        //    var checkRoleResult = await CheckAdminRole();
        //    if (checkRoleResult != null) return checkRoleResult;

        //    var salon = await _context.Salons.Include(s => s.Employees).FirstOrDefaultAsync(s => s.Id == id);
        //    if (salon == null) return NotFound();

        //    return View(salon);
        //}

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> EditSalon(Salon salon, IFormFile logoFile)
        //{
        //    var checkRoleResult = await CheckAdminRole();
        //    if (checkRoleResult != null) return checkRoleResult;

        //    var existingSalon = await _context.Salons
        //        .Include(s => s.Employees)
        //        .Include(s => s.WorkingHours)
        //        .FirstOrDefaultAsync(s => s.Id == salon.Id);

        //    if (existingSalon == null) return NotFound();

        //    if (salon.WorkingHours == null || !salon.WorkingHours.Any())
        //    {
        //        salon.WorkingHours = existingSalon.WorkingHours;
        //    }

        //    existingSalon.Name = salon.Name;
        //    existingSalon.Location = salon.Location;
        //    existingSalon.Address = salon.Address;
        //    existingSalon.WorkingHours = salon.WorkingHours;
        //    existingSalon.GoogleMapsIframe = salon.GoogleMapsIframe;

        //    // Ako je poslana nova slika logotipa
        //    if (logoFile != null && logoFile.Length > 0)
        //    {
        //        var fileExtension = Path.GetExtension(logoFile.FileName);
        //        var fileName = Guid.NewGuid().ToString() + fileExtension;

        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", fileName);
        //        var directory = Path.GetDirectoryName(filePath);
        //        if (!Directory.Exists(directory))
        //        {
        //            Directory.CreateDirectory(directory);
        //        }

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await logoFile.CopyToAsync(fileStream);
        //        }

        //        // Spremanje nove relativne putanje
        //        existingSalon.ImagePath = "images/logos/" + fileName;
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Salons));
        //    }

        //    return View(salon);
        //}


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddService(int salonId, string serviceName, decimal price)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            if (!string.IsNullOrWhiteSpace(serviceName))
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

                if (ModelState.IsValid)
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Salons");
                }
            }

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
                .Include(s => s.Employees) // Preuzimamo postojeće zaposlenike
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salon == null)
                return NotFound();

            // Dodajemo zaposlenike za salon u ViewBag za prikaz na formi
            ViewBag.Employees = await _context.Employees.ToListAsync();

            return View(salon); // Vraćamo pogled za uređivanje salona
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSalon(Salon salon)
        {
            var checkRoleResult = await CheckAdminRole();
            if (checkRoleResult != null) return checkRoleResult;

            // Provjerite postoji li salon u bazi
            var existingSalon = await _context.Salons
                .Include(s => s.Employees) // Učitaj postojeće zaposlenike
                .FirstOrDefaultAsync(s => s.Id == salon.Id);

            if (existingSalon == null)
                return NotFound();

            // Očistimo stare zaposlenike
            existingSalon.Employees.Clear();

            // Dodajemo nove zaposlenike koji su poslani iz forme
            if (salon.Employees != null)
            {
                foreach (var employee in salon.Employees)
                {
                    if (!string.IsNullOrWhiteSpace(employee.Name)) // Sprečavamo dodavanje praznih polja
                    {
                        existingSalon.Employees.Add(new Employee
                        {
                            Name = employee.Name,
                            SalonId = existingSalon.Id
                        });
                    }
                }
            }

            // Spremi promjene u bazu
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Salons)); // Preusmjeri na listu salona
        }



    }
}
