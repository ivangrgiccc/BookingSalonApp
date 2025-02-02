using System.ComponentModel.DataAnnotations;
namespace BookingSalonApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi lozinku")]
        [Compare("Password", ErrorMessage = "Lozinka se ne poklapa sa unesenom lozinkom.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; } 
    }
}
