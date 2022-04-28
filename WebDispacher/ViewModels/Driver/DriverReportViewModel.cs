namespace WebDispacher.ViewModels.Driver
{
    public class DriverReportViewModel
    {
        public int Id { get; set; }
        public int IdDriver { get; set; }
        public int IdCompany { get; set; }
        public string Comment { get; set; }
        public string FullName { get; set; }
        public string English { get; set; }
        public string ReturnedEquipmen { get; set; }
        public string WorkingEfficiency { get; set; }
        public string EldKnowledge { get; set; }
        public string DrivingSkills { get; set; }
        public string PaymentHandling { get; set; }
        public string AlcoholTendency { get; set; }
        public string DrugTendency { get; set; }
        public string Terminated { get; set; }
        public string DotViolations { get; set; }
        public string NumberOfAccidents { get; set; }
        public string Experience { get; set; }
        public string DriversLicenseNumber { get; set; }
        public string DateRegistration { get; set; }
        public string DateFired { get; set; }
    }
}