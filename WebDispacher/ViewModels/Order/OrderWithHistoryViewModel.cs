using System.Collections.Generic;

namespace WebDispacher.ViewModels.Order
{
    public class OrderWithHistoryViewModel
    {
        public DaoModels.DAO.Models.Order Order { get; set; }

        public List<HistoryOrderGroup> Actions { get; set; }
    }
}
