using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDispatch.Models;
using MDispatch.Models.ModelDataBase;
using MDispatch.NewElement.DataBase;
using SQLite;
using Xamarin.Forms;

namespace MDispatch.Service
{
    public class DataBaseContext
    {
        private SQLiteAsyncConnection _connection;

        public DataBaseContext()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            _connection.CreateTableAsync<FolderOffline>();
        }

        public async Task AddPhotoInspection(FolderOffline folderOffline)
        {
            await _connection.InsertAsync(folderOffline);
        }

        internal async Task<List<FolderOffline>> GetFolderOfflines()
        {
            return await _connection.Table<FolderOffline>().ToListAsync();
        }

        internal async Task DeleteFolderOfflines(int idFolderOffline)
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
