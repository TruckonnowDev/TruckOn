using System;
using SQLite;

namespace MDispatch.NewElement.DataBase
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
