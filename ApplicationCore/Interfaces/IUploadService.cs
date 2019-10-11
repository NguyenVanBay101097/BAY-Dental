using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadBinaryAsync(string base64, string fileName = "");
    }
}
