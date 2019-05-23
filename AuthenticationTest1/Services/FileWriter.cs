using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Services
{
    public class FileWriter : IFileWriter
    {
        public async Task<(bool success, string fileName)> WriteFile(IFormFile file, string fileName)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\db_files", fileName);
                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }

            return (true, fileName);
        }
        public async Task<(bool success, string fileName)> WriteFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            var tempFileName = Guid.NewGuid().ToString() + extension;

            (bool, string) result = await WriteFile(file, tempFileName);

            return (result.Item1, result.Item2);
        }
    }
}
