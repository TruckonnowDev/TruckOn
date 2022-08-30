namespace WebDispacher.Models
{
    public class ResponseStripe
    {
        public bool IsError { get; set; }
        public object Content { get; set; }
        public string Message { set; get; }
    }
}
