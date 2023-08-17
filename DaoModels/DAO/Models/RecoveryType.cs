using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class RecoveryType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PasswordRecovery> PasswordRecoveries { get; set; }
    }
}
