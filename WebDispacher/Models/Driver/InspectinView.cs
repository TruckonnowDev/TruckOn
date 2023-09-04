using System;

namespace WebDispacher.Models.Driver
{
    public class InspectinView
    {
        public int Id { get; set; }
        public DateTime DateTimeInspection { get; set; }
        public string TrailerInformation { get; set; }
        public string TruckInformation { get; set; }
        public string NameDriver { get; set; }
    }
}