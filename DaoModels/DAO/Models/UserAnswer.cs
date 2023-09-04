namespace DaoModels.DAO.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? DriverQuestionnaireId { get; set; }
        public DriverQuestionnaire DriverQuestionnaire { get; set; }
        public string TextAnswer { get; set; }
    }
}
