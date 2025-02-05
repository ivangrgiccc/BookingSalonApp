using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookingSalonApp.Models;

namespace BookingSalonApp.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti pozitivna i veća od nule.")]
        public decimal Price { get; set; }

        public int SalonId { get; set; }
        public Salon Salon { get; set; }
        public ICollection<ReservationService> ReservationServices { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}