using System;

namespace DaoModels.DAO.Models
{
    public class PasswordRecovery
    {
        public int Id { get; set; }
        public string EntityRecoveryId { get; set; }
        public DateTime DateTimeAction { get; set; }
        public string Token { get; set; }
        public int RecoveryTypeId { get; set; }
        public RecoveryType RecoveryType { get; set; }
    }
}
