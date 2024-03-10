using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class TrailerStatusTheme : StatusTheme
    {
        public ICollection<TrailerStatus> TrailerStatuses { get; set; }
    }
}