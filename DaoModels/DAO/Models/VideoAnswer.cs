namespace DaoModels.DAO.Models
{
    public class VideoAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? DriverQuestionnaireId { get; set; }
        public DriverQuestionnaire DriverQuestionnaire { get; set; }
        public int VideoId { get; set; }
        public Video Video { get; set; }
    }
}
