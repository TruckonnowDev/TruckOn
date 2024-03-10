using WebDispacher.Constants;

namespace WebDispacher.ViewModels.Resources
{
    public class ResourceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PositionIndex { get; set; }
        public string UrlPicture { get; set; } = ResourcesConstants.UrlToPicture;
    }
}
