using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDispatch.NewElement.Directory
{
    public interface IMediaService
    {
        Task<bool> SaveImageFromByte(byte[] imageByte, string filename);
    }
}