using System;

namespace DaoModels.DAO.Models
{
    public class UserQuestionnaire
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateTimeCompleted { get; set; }
    }
}
