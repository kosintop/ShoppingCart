using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public static class ImageUploader
    {
        private static readonly string UPLOAD_ROOT = "wwwroot";

        public static async Task<string> UploadImage(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), UPLOAD_ROOT, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(uniqueFileName);
        }
    }
}
