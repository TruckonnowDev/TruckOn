using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DaoModels.DAO.Models
{
    public class Video
    {
        public int Id { get; set; }
        public int VideoTypeId { get; set; }
        public VideoType VideoType { get; set; }
        public string VideoPath { get; set; }
        public string VideoUrl { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public DateTime DateTimeUpload { get; set; }
        public string VideoBase64
        {
            set
            {
                try
                {
                    byte[] photoInArrayByte = Convert.FromBase64String(value);
                    if (!Directory.Exists(VideoPath))
                    {
                        string pathTmp = VideoPath.Remove(VideoPath.LastIndexOf("/"));
                        Directory.CreateDirectory(pathTmp);
                    }
                    using (var imageFile = new FileStream(VideoPath, FileMode.Create))
                    {
                        imageFile.Write(photoInArrayByte, 0, photoInArrayByte.Length);
                        imageFile.Flush();
                    }
                }
                catch (Exception e)
                {

                }
            }
            get
            {
                if (VideoPath != null)
                {
                    try
                    {
                        string tmpJson = JsonConvert.SerializeObject(File.ReadAllBytes(VideoPath));
                        tmpJson = tmpJson.Replace("\"", string.Empty);
                        return tmpJson;
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
