namespace DaoModels.DAO.Models
{
    public class Dispatcher
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int DispatcherTypeId { get; set; }
        public DispatcherType DispatcherType { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
    }
}
