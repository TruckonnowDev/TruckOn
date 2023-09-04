using System;

namespace DaoModels.DAO.Models
{
    public class DriverQuestionnaire
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateTimeCompleted { get; set; }
    }
}
