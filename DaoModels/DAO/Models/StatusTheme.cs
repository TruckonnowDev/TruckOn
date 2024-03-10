namespace DaoModels.DAO.Models
{
    public class StatusTheme
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? BackgroundColorId { get; set; }
        public Color BackgroundColor { get; set; }
        public int? TextColorId { get; set; }
        public Color TextColor { get; set; }
    }
}