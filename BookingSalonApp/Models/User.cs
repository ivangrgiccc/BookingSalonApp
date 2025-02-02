using Microsoft.AspNetCore.Identity;  
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BookingSalonApp.Models
{
    public class User : IdentityUser 
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Prezime je obavezno.")]
        public string LastName { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
