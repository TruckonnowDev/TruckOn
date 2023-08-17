using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class Questionnaire
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
