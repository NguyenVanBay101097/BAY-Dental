using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IUploadService
    {
        Task<UploadResult> UploadBinaryAsync(string base64, string fileName);
    }
}
