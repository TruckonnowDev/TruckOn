using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class ViewUserMailQuestion
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int UserMailQuestionId { get; set; }
        public UserMailQuestion UserMailQuestion { get; set; }
        public DateTime DateTimeAction { get; set; }
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
    }
}