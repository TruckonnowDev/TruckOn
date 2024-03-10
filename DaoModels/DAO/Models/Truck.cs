using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime? PlateExpires { get; set; }
        public DateTime? AnnualIns { get; set; }
        public string VIN { get; set; }
        public string Owner { get; set;}
        public string Plate { get; set;}
        public string Color { get; set;}
        public int? TruckTypeId { get; set;}
        public TruckType TruckType { get; set;}
        public int? TruckStatusId { get; set; }
        public TruckStatus TruckStatus { get; set;}
        public LocationType LocationType { get; set;}
		public string LocationAddress { get; set; }
        public string State { get; set; }
        public int TruckGroupId { get; set; }
        public TruckGroup TruckGroup { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public ICollection<DocumentTruck> Documents { get; set; }
        public ICollection<DriverInspection> Inspections { get; set; }
    }
}