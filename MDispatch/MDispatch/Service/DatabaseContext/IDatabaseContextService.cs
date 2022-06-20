using MDispatch.Models.ModelDataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDispatch.Service.DatabaseContext
{
    public interface IDatabaseContextService
    {
        Task AddPhotoInspection(FolderOffline folderOffline);
        Task<List<FolderOffline>> GetFolderOfflines();
        Task DeleteFolderOfflines(int idFolderOffline);
    }
}
