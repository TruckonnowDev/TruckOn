﻿namespace DaoModels.DAO.Models
{
    public class PhotoAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? UserQuestionnaireId { get; set; }
        public UserQuestionnaire UserQuestionnaire { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
    }
}
