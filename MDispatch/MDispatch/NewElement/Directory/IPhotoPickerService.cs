using System.IO;
using System.Threading.Tasks;

namespace MDispatch.NewElement.Directory
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}