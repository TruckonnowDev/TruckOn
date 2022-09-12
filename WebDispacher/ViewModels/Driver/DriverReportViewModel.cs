using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Driver
{
    public class DriverReportViewModel
    {
        public int Id { get; set; }
        public int IdDriver { get; set; }
        public int IdCompany { get; set; }
        
        [Required]
        public string Comment { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        public string English { get; set; }
        
        [Required]
        public string ReturnedEquipmen { get; set; }
        
        [Required]
        public string WorkingEfficiency { get; set; }
        
        [Required]
        public string EldKnowledge { get; set; }
        
        [Required]
        public string DrivingSkills { get; set; }
        
        [Required]
        public string PaymentHandling { get; set; }
        
        [Required]
        public string AlcoholTendency { get; set; }
        
        [Required]
        public string DrugTendency { get; set; }
        
        [Required]
        public string Terminated { get; set; }
        
        [Required]
        public string DotViolations { get; set; }
        
        [Required]
        public string NumberOfAccidents { get; set; }
        
        public string Experience { get; set; }
        
        [Required]
        public string DriversLicenseNumber { get; set; }
        public string DateRegistration { get; set; }
        public string DateFired { get; set; }
    }
}