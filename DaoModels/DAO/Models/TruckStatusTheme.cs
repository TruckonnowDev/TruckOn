using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class TruckStatusTheme : StatusTheme
    {
        public ICollection<TruckStatus> TruckStatuses { get; set; }
    }
}