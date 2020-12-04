using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UploadService: IUploadService
    {
        private readonly string _serverUploadApi;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UploadService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _serverUploadApi = config.GetValue<string>("ServerUploadApi");
        }

        public async Task<UploadResult> UploadBinaryAsync(string base64, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = Path.GetRandomFileName();

            var appId = string.Empty;
            var host = _httpContextAccessor.HttpContext.Request.Host.Value;
            if (!string.IsNullOrWhiteSpace(host))
            {
                appId = host.Split('.')[0];
                if (appId.Contains("localhost"))
                    appId = "localhost";
            }

            appId = appId.Trim().ToLower();

            var handler = new HttpClientHandler
            {
                UseProxy = true
            };

            handler.ServerCertificateCustomValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("__TPosId", appId);

                using (Stream contentStream = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    using (var formData = new MultipartFormDataContent())
                    {
                        HttpContent fileStreamContent = new StreamContent(contentStream);

                        formData.Add(fileStreamContent, "files", fileName);

                        var response = client.PostAsync(_serverUploadApi, formData).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            return null;
                        }

                        var stringMessage = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(stringMessage))
                        {
                            var result = JsonConvert.DeserializeObject<IEnumerable<UploadResult>>(stringMessage);
                            return result.FirstOrDefault();
                        }

                        return null;
                    }
                }
            }
        }

        public async Task<IEnumerable<UploadResult>> UploadBinaryAsync(IList<IFormFile> files)
        {
            var appId = string.Empty;
            var host = _httpContextAccessor.HttpContext.Request.Host.Value;
            if (!string.IsNullOrWhiteSpace(host))
            {
                appId = host.Split('.')[0];
                if (appId.Contains("localhost"))
                    appId = "localhost";
            }

            appId = appId.Trim().ToLower();

            var handler = new HttpClientHandler
            {
                UseProxy = true
            };

            handler.ServerCertificateCustomValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("__TPosId", appId);

                using (var formData = new MultipartFormDataContent())
                {
                    var memoryStreamList = new List<MemoryStream>();
                    foreach (var file in files)
                    {
                        //var memoryStream = new MemoryStream();
                        //file.CopyTo(memoryStream);

                        HttpContent fileContent = new StreamContent(file.OpenReadStream());
                        formData.Add(fileContent, "files", file.FileName);

                        //memoryStreamList.Add(memoryStream);
                    }

                    var response = client.PostAsync(_serverUploadApi, formData).Result;

                    //foreach (var stream in memoryStreamList)
                    //    stream.Dispose();

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var stringMessage = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(stringMessage))
                    {
                        var result = JsonConvert.DeserializeObject<IEnumerable<UploadResult>>(stringMessage);
                        return result;
                    }


                    return null;
                }
            }
        }
    }

    public class UploadResult
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }
}
