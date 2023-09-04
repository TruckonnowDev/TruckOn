using System;

namespace WebDispacher.ViewModels.Company
{
    public class AdminAnswerViewModel
    {
        public int QuestionId { get; set; }
        public string AdminName { get; set; }
        public string Message { get; set; }
        public DateTime DateTimeAnswer { get; set; }
    }
}
