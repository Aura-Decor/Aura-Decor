using AuraDecor.Core.Services.Contract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Servicies
{
    public static class FileUploadService
    {
        private static readonly string _uploadPath;
        static FileUploadService()
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public static async Task<string> UploadAsync(IFormFile file)
        {
            List<string> ValidExtentions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };
            string extention = Path.GetExtension(file.FileName).ToLower();
            if (file is null || file.Length == 0)
            {
                return "No file uploaded.";
            }
            if (!ValidExtentions.Contains(extention))
            {
                return $"Invalid file type. Allowed types: {string.Join(", ", ValidExtentions)}";
            }
            long size = file.Length;
            if (size > (7 * 1024 * 1024))
            {
                return "File size exceeds 7MB limit.";
            }
            string fileName = $"{Guid.NewGuid()}{extention}";
            string filePath = Path.Combine(_uploadPath, fileName);
            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return fileName;
        }
    }
}
