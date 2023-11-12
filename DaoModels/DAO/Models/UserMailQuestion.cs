using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class UserMailQuestion
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Message { get; set; }
        public MailStatus MailStatus { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public List<AdminAnswer> AdminAnswers { get; set; }
        public List<ViewUserMailQuestion> Views { get; set; }
    }
}
