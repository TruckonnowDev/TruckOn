namespace DaoModels.DAO.DTO
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string DateRegistration { get; set; }
        public string Type { get; set; }
        public string SubscriptionStatus { get; set; }
        public string SubscriptionName { get; set; }
    }
}
