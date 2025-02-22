using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Services.Contract
{
    public interface IFileUploadService
    {
         Task<string> UploadAsync(IFormFile file);
    }
}
