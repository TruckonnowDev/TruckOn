using System.ComponentModel.DataAnnotations.Schema;

namespace DaoModels.DAO.Models
{
    public class StatusWidget
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int CountItemsInStatus { get; set; }
        public int? StatusThemeId { get; set; }
        public StatusTheme StatusTheme { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}