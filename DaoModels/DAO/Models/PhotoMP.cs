using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace DaoModels.DAO.Models
{
    public class PhotoMP
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PhotoListMPId { get; set; }
        public PhotoListMP PhotoListMP { get; set; }
        public int PhotoTypeId { get; set; }
        public PhotoType PhotoType { get; set; }
        public string PhotoPath { get; set; }
        public string PhotoUrl { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public DateTime DateTimeUpload { get; set; }
        [NotMapped]
        public string Base64
        {
            set
            {
                try
                {
                    byte[] photoInArrayByte = Convert.FromBase64String(value);
                    if (!Directory.Exists(PhotoPath))
                    {
                        string pathTmp = PhotoPath.Remove(PhotoPath.LastIndexOf("/"));
                        Directory.CreateDirectory(pathTmp);
                    }
                    using (var imageFile = new FileStream(PhotoPath, FileMode.Create))
                    {
                        imageFile.Write(photoInArrayByte, 0, photoInArrayByte.Length);
                        imageFile.Flush();
                    }
                }
                catch (Exception)
                {

                }
            }
            get
            {
                if (PhotoPath != null)
                {
                    try
                    {
                        string tmpJson = JsonConvert.SerializeObject(File.ReadAllBytes(PhotoPath));
                        tmpJson = tmpJson.Replace("\"", "");
                        return tmpJson;
                    }
                    catch(Exception)
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
