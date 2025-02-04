using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Data;
using BookingSalonApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalonApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult MyReservations(string userId)
        {
            var reservations = _context.Reservations
                .Include(r => r.Salon)
                .Include(r => r.Employee)
                .Include(r => r.ReservationServices)
                .ThenInclude(rs => rs.Service)
                .Where(r => r.UserId == userId)
                .ToList();

            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int salonId)
        {
            var salon = await _context.Salons
                .Include(s => s.Employees)
                .Include(s => s.Services)
                .Include(s => s.WorkingHours)
                .FirstOrDefaultAsync(s => s.Id == salonId);

            if (salon == null)
                return NotFound();

            var model = new BookingViewModel
            {
                SalonId = salon.Id,
                SalonName = salon.Name,
                Employees = salon.Employees.ToList(),
                Services = salon.Services.ToList(),
                WorkingHours = salon.WorkingHours.ToList(),
                UserId = int.TryParse(_userManager.GetUserId(User), out var userId) ? (int?)userId : null
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Book(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "Korisnik nije autentificiran.");
                    return View(model);
                }

                if (DateTime.TryParse(model.Date, out var selectedDate))
                {
                    var salon = await _context.Salons
                        .Include(s => s.WorkingHours)
                        .FirstOrDefaultAsync(s => s.Id == model.SalonId);

                    if (salon == null || !salon.WorkingHours.Any(w => w.DayOfWeek == selectedDate.DayOfWeek && w.StartTime <= selectedDate.TimeOfDay && w.EndTime >= selectedDate.TimeOfDay))
                    {
                        ModelState.AddModelError("", "Odabrani termin nije unutar radnog vremena salona.");
                        return View(model);
                    }

                    var reservation = new Reservation
                    {
                        UserId = userId,
                        EmployeeId = model.EmployeeId ?? 0,
                        SalonId = model.SalonId,
                        Date = selectedDate
                    };

                    _context.Reservations.Add(reservation);
                    await _context.SaveChangesAsync();

                    if (model.SelectedServices != null)
                    {
                        foreach (var serviceId in model.SelectedServices)
                        {
                            var reservationService = new ReservationService
                            {
                                ReservationId = reservation.Id,
                                ServiceId = serviceId
                            };
                            _context.ReservationServices.Add(reservationService);
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("MyReservations", new { userId = reservation.UserId });
                }
                else
                {
                    ModelState.AddModelError("", "Neispravan termin.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ReservationServices)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            _context.ReservationServices.RemoveRange(reservation.ReservationServices);
            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyReservations", new { userId = reservation.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int salonId, int employeeId, DateTime date)
        {
            var salon = await _context.Salons
                .Include(s => s.WorkingHours)
                .FirstOrDefaultAsync(s => s.Id == salonId);

            if (salon == null)
                return NotFound();

            // Provera da salon ima radno vreme
            if (salon.OpeningTime == TimeSpan.Zero || salon.ClosingTime == TimeSpan.Zero)
            {
                return Json(new List<string>()); // Ako nema radnog vremena, nema dostupnih termina
            }

            var reservations = await _context.Reservations
                .Where(r => r.EmployeeId == employeeId && r.Date.Date == date.Date)
                .ToListAsync();

            var availableSlots = new List<string>();

            var workStartTime = date.Date.Add(salon.OpeningTime);
            var workEndTime = date.Date.Add(salon.ClosingTime);

            var allSlots = new List<DateTime>();
            for (var time = workStartTime; time < workEndTime; time = time.AddMinutes(40))
            {
                allSlots.Add(time);
            }

            foreach (var slot in allSlots)
            {
                if (!reservations.Any(r => r.Date == slot))
                {
                    availableSlots.Add(slot.ToString("yyyy-MM-ddTHH:mm"));
                }
            }

            return Json(availableSlots);
        }

    }
}
