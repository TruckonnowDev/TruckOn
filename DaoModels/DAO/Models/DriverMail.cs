using DaoModels.DAO.Enum;
using System;

namespace DaoModels.DAO.Models
{
    public class DriverMail
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        public string ReceivedEmail { get; set; }
        public string Message { get; set; }
        public MailStatus MailStatus { get; set; }
        public DateTime DateTimeReceived { get; set; }
    }
}