using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class DispatcherType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Dispatcher> Dispatchers { get; set; }
    }
}
