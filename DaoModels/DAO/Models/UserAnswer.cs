namespace DaoModels.DAO.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? UserQuestionnaireId { get; set; }
        public UserQuestionnaire UserQuestionnaire { get; set; }
        public string TextAnswer { get; set; }
    }
}
