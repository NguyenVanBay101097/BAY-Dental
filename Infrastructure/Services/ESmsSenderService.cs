using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ESmsSenderService
    {
        private string _brandname;
        private string _apiKey;
        private string _serectKey;
        public ESmsSenderService(string brandname, string apiKey, string serectKey)
        {
            _brandname = brandname;
            _apiKey = apiKey;
            _serectKey = serectKey;
        }

        public async Task<ESMSSendMessageResponseModel> SendAsync(string phone, string content)
        {
            var url = "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/";
            var client = new HttpClient();

            var data = new ESMSSendMessageRequestModel
            {
                Phone = phone,
                ApiKey = _apiKey,
                Brandname = _brandname,
                Content = content,
                SecretKey = _serectKey,
                SmsType = "2",
                IsUnicode = 1,
            };

            var jsonObject = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(url, stringContent);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var sendResonse = JsonConvert.DeserializeObject<ESMSSendMessageResponseModel>(response);

                return sendResonse;
            }
          
            return null;
        }

        public class ESMSSendMessageRequestModel
        {
            public string Phone { get; set; }
            public string Content { get; set; }
            public string ApiKey { get; set; }
            public string SecretKey { get; set; }
            public string SmsType { get; set; }
            public string Brandname { get; set; }
            public int IsUnicode { get; set; }
            public string Sandbox { get; set; }
        }

        public class ESMSSendMessageResponseModel
        {
            public string CodeResult { get; set; }
            public string CountRegenerate { get; set; }
            public string SMSID { get; set; }
            public string Message { get; set; }
        }
    }
}
