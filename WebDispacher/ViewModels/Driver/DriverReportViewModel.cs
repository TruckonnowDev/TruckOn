using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Driver
{
    public class DriverReportViewModel
    {
        public int Id { get; set; }
        public int IdDriver { get; set; }
        public int IdCompany { get; set; }
        
        [Required(ErrorMessage = "CommentRequired")]
        public string Comment { get; set; }
        
        [Required(ErrorMessage = "FullNameRequired")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "EnglishRequired")]
        public string English { get; set; }
        
        [Required(ErrorMessage = "ReturnedEquipentRequired")]
        public string ReturnedEquipmen { get; set; }
        
        [Required(ErrorMessage = "WorkingEfficiencyRequired")]
        public string WorkingEfficiency { get; set; }
        
        [Required(ErrorMessage = "EldKnowledgeRequired")]
        public string EldKnowledge { get; set; }
        
        [Required(ErrorMessage = "DrivingSkillsRequired")]
        public string DrivingSkills { get; set; }
        
        [Required(ErrorMessage = "PaymentHadlingRequired")]
        public string PaymentHandling { get; set; }
        
        [Required(ErrorMessage = "AlcoholTendencyRequired")]
        public string AlcoholTendency { get; set; }
        
        [Required(ErrorMessage = "DrugTendencyRequired")]
        public string DrugTendency { get; set; }
        
        [Required(ErrorMessage = "TerminatedRequired")]
        public string Terminated { get; set; }
        
        [Required(ErrorMessage = "DotViolationsRequired")]
        public string DotViolations { get; set; }
        
        [Required(ErrorMessage = "NumberOfAccidentsRequired")]
        public string NumberOfAccidents { get; set; }
        
        public string Experience { get; set; }
        
        [Required(ErrorMessage = "DriversLicenseNumberRequired")]
        public string DriversLicenseNumber { get; set; }
        public string DateRegistration { get; set; }
        public string DateFired { get; set; }
    }
}