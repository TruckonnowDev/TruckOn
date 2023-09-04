using DaoModels.DAO.Enum;

namespace DaoModels.DAO.Models
{
    public class PhotoDriverInspection
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public TransportType Type { get; set; }
        public int DriverInspectionId { get; set; }
        public DriverInspection DriverInspection { get; set; }
        public int IndexPhoto { get; set; }
    }
}
