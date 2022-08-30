namespace DaoModels.DAO.Models
{
    public class PasswordRecovery
    {
        public int Id { get; set; }
        public int IdDriver { get; set; }
        public int IdUser { get; set; }
        public string Date { get; set; }
        public string Token { get; set; }
    }
}