namespace DaoModels.DAO.Models
{
    public class PhotoAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? DriverQuestionnaireId { get; set; }
        public DriverQuestionnaire DriverQuestionnaire { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
    }
}
