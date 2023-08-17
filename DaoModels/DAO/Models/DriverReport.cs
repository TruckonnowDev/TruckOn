using System;

namespace DaoModels.DAO.Models
{
    public class DriverReport
    {
        public int Id { get; set; }
        public Driver Driver { get; set; }
        public string AlcoholTendency { get; set; }
        public string DriverSkills { get; set; }
        public string DrugTendency { get; set; }
        public string EldKnowledge { get; set; }
        public string English { get; set; }
        public string Experience { get; set; }
        public string PaymentHandling { get; set; }
        public string ReturnEquipment { get; set; }
        public string Terminated { get; set; }
        public string WorkEffeciency { get; set; }
        public string DotViolations { get; set; }
        public string NumberAccidents { get; set; }
        public string Comment { get; set; }
        public DateTime DateFired { get; set; }
        
    }
}
