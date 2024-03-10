using System;

namespace WebDispacher.ViewModels
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime UploadedTime { get; set; }
    }
}