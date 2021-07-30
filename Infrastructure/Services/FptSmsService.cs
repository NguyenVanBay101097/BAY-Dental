using ApplicationCore.Utilities;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FptSmsService
    {
        public HttpClient Client { get; }
        public FptAuthConfig AuthConfig { get; set; }

        private string _accessToken { get; set; }

        public FptSmsService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://app.sms.fpt.net/");
            Client = client;
        }

        public async Task<FptSmsServiceGetAccessTokenResult> GetAccessToken()
        {
            //thành công trả về access token
            //thất bại trả về object error: error code, error description

            if (AuthConfig == null)
                throw new ArgumentNullException();
          
            try
            {
                var data = AuthConfig.GetAuthData();
                var jsonObject = JsonConvert.SerializeObject(data);
                var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("/oauth2/token", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<JObject>(content);

                    return new FptSmsServiceGetAccessTokenResult
                    {
                        AccessToken = (string)result["access_token"]
                    };
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<JObject>(content);

                    return new FptSmsServiceGetAccessTokenResult
                    {
                        Error = (int)result["error"],
                        ErrorDescription = (string)result["error_description"]
                    };
                }
            }
            catch
            {
                return new FptSmsServiceGetAccessTokenResult
                {
                    Error = -1,
                    ErrorDescription = "Không xác định"
                };
            }
        }

        public async Task<FptSmsServiceSendSmsResult> SendSms(string brandname, string phone, string message)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                var getAccessTokenResult = await GetAccessToken();
                if (getAccessTokenResult.Error == 0)
                {
                    _accessToken = getAccessTokenResult.AccessToken;
                }
                else
                {
                    return new FptSmsServiceSendSmsResult
                    {
                        Error = getAccessTokenResult.Error,
                        Error_Description = getAccessTokenResult.ErrorDescription
                    };
                }
            }

            var sessionId = StringUtils.RandomString(32);
            var data = new
            {
                Phone = phone,
                access_token = this._accessToken,
                BrandName = brandname,
                Message = message.EncodeBase64(),
                session_id = sessionId
            };

            var jsonObj = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");

            try
            {
                var response = await Client.PostAsync("/api/push-brandname-otp", stringContent);
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FptSmsServiceSendSmsResult>(content);
            }
            catch
            {
                return new FptSmsServiceSendSmsResult
                {
                    Error = -1,
                    Error_Description = "Không xác định"
                };
            }
        }
    }

    public class FptSmsServiceGetAccessTokenResult
    {
        public string AccessToken { get; set; }
        public int Error { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class FptSmsServiceSendSmsResult
    {
        /// <summary>
        /// Id của tin brandname gửi đi. 
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Tên brandname 
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// Số điện thoại gửi tin nhắn. Định dạng 84xxxx
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Nội dung tin nhắn gửi đi
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ID của đối tác
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Nhà mạng của thuê bao khách hàng.
        /// </summary>
        public string Telco { get; set; }

        public int Error { get; set; }

        public string Error_Description { get; set; }
    }
}
