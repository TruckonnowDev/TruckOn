using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
        public ICollection<VideoAnswer> VideoAnswers { get; set; }
        public ICollection<PhotoAnswer> PhotoAnswers { get; set; }
    }
}
