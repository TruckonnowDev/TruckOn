using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models.Settings
{
    public class ProfileSettings
    {
        public int Id { get; set; }
        public int IdCompany { get; set; }
        public int IdTr { get; set; }
        public string TypeTransportVehikle { get; set; }
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public TransportVehicle TransportVehicle { get; set; }
    }
}
