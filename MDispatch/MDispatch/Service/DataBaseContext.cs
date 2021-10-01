using System;
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
            //_connection.CreateTableAsync<Contact>();
        }
    }
}
