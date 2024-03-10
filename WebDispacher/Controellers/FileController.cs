using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.ViewModels;
using Microsoft.AspNetCore.Authorization;
using WebDispacher.Constants.Identity;

namespace WebDispacher.Controellers
{
    public class FileController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IWebHostEnvironment _environment;

        public FileController(
            IUserService userService,
            ICompanyService companyService,
            IWebHostEnvironment environment) : base(userService)
        {
            this.companyService = companyService;
            _environment = environment;
        }

        [HttpPost]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public IActionResult UploadFile(IFormFile file)
        {
            var uploadsFolderPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            if (file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                var fileModel = new FileModel
                {
                    FileName = fileName,
                    FilePath = filePath,
                    UploadedTime = DateTime.Now,
                };

                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    file.CopyTo(fileStream); 
                    using var image = System.Drawing.Image.FromStream(fileStream);
                    fileModel.Width = image.Width;
                    fileModel.Height = image.Height;
                }

                return PartialView("_Preview", fileModel);
            }

            return BadRequest("No file uploaded");
        }

        [HttpPost]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public IActionResult DeleteFile( string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);

                    return Ok();
                }
                else
                {
                    return NotFound($"File '{fileName}' not found");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки при удалении файла
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }

    }
}