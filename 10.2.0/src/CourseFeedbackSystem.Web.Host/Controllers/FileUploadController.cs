using Abp.AspNetCore.Mvc.Authorization;
using CourseFeedbackSystem.Authorization;
using CourseFeedbackSystem.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Web.Host.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Feedbacks)]
    public class FileUploadController : CourseFeedbackSystemControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileUploadController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Validate file extension
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (string.IsNullOrEmpty(fileExtension) || !Array.Exists(allowedExtensions, ext => ext == fileExtension))
            {
                return BadRequest("Invalid file type. Only PDF, JPG, and PNG files are allowed.");
            }

            // Validate file size (max 10MB)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
            {
                return BadRequest("File size exceeds the maximum limit of 10MB.");
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "feedbacks");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique file name
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return relative URL
                var fileUrl = $"/uploads/feedbacks/{uniqueFileName}";
                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult DownloadFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return BadRequest("File URL is required.");
            }

            // Security: Ensure the file is within the uploads directory
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, fileUrl.TrimStart('/'));
            
            if (!filePath.StartsWith(Path.Combine(_hostingEnvironment.WebRootPath, "uploads")))
            {
                return BadRequest("Invalid file path.");
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);
            var contentType = GetContentType(fileName);

            return File(fileBytes, contentType, fileName);
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }
    }
}

