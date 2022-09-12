using System.ComponentModel.DataAnnotations;

namespace WebDispacher.Models
{
    public class DriverReportModel
    {
        public int Id { get; set; }
        
        [Required]
        public string NumberOfAccidents { get; set; }
        
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
        
        public string Experience { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string DotViolations { get; set; }
    }
}