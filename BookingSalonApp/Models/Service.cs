﻿using System;
using System.Collections.Generic;
using BookingSalonApp.Models;

namespace BookingSalonApp.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int SalonId { get; set; }
        public Salon Salon { get; set; }
        public ICollection<ReservationService> ReservationServices { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}