using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Services
{
    public interface IFileWriter
    {
        Task<(bool success, string fileName)> WriteFile(IFormFile file);
        Task<(bool success, string fileName)> WriteFile(IFormFile file, string fileName);
    }
}
