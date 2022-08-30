namespace WebDispacher.ViewModels.Driver
{
    public class DriverViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string TrailerCapacity { get; set; }
        public string DriversLicenseNumber { get; set; }
        public string DateRegistration { get; set; }
        public int CompanyId { get; set; }
    }
}