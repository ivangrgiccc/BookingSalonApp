namespace BookingSalonApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int SalonId { get; set; }
        public Salon Salon { get; set; }


        public List<Reservation> Reservations { get; set; }
    }
}
