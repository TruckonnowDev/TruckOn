using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class Trailer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string HowLong { get; set; }
        public string Vin { get; set; }
        public string Owner { get; set; }
        public string Color { get; set; }
        public string Plate { get; set; }
        public DateTime? PlateExpires { get; set; }
        public DateTime? AnnualIns { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public int? TrailerTypeId { get; set; }
        public TrailerType TrailerType { get; set; }
        public int TrailerGroupId { get; set; }
        public TrailerGroup TrailerGroup { get; set; }
        public int? TrailerStatusId { get; set; }
        public TrailerStatus TrailerStatus { get; set; }
        public LocationType LocationType { get; set; }
        public string LocationAddress { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public ICollection<DocumentTrailer> Documents { get; set; }
        public ICollection<DriverInspection> Inspections { get; set; }
    }
}