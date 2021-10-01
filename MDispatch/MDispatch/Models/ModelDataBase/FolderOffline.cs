using System;
using MDispatch.Models.Enum;
using SQLite;

namespace MDispatch.Models.ModelDataBase
{
    public class FolderOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string IdShiping { get; set; }
        public string IdVech { get; set; }
        public int Index { get; set; }
        public FolderOflineType FolderOflineType { get; set; }
        public string Json { get; set; }
    }
}
