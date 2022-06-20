using MDispatch.Models.ModelDataBase;
using MDispatch.NewElement.DataBase;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.DatabaseContext
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private SQLiteAsyncConnection _connection;

        public DatabaseContextService()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            _connection.CreateTableAsync<FolderOffline>();
        }

        public async Task AddPhotoInspection(FolderOffline folderOffline)
        {
            await _connection.InsertAsync(folderOffline);
        }

        public async Task<List<FolderOffline>> GetFolderOfflines()
        {
            return await _connection.Table<FolderOffline>().ToListAsync();
        }

        public async Task DeleteFolderOfflines(int idFolderOffline)
        {
            try
            {
                await _connection.DeleteAsync<FolderOffline>(idFolderOffline);
            }
            catch
            {

            }
        }
    }
}
