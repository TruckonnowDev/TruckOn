using System;

namespace DaoModels.DAO.Models
{
    public class AdminAnswer
    {
        public int Id { get; set; }
        public string AdminName { get; set; }
        public string Message { get; set; }
        public DateTime DateTimeAnswer { get; set; }
        public int UserMailQuestionId { get; set; }
        public UserMailQuestion UserMailQuestion { get; set; }
    }
}
