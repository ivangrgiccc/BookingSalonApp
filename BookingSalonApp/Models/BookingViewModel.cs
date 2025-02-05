using BookingSalonApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingSalonApp.Models
{
    public class BookingViewModel
    {
        [Required]
        public int SalonId { get; set; }

        public string SalonName { get; set; }

        [Required(ErrorMessage = "Molimo odaberite zaposlenika.")]
        public int? EmployeeId { get; set; }

        [Required(ErrorMessage = "Molimo odaberite usluge.")]
        public List<int> SelectedServices { get; set; }

        [Required(ErrorMessage = "Molimo odaberite datum.")]
        public DateTime Date { get; set; }

        public string TimeSlot { get; set; }

        public int? UserId { get; set; }
    }
}
