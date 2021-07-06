using ApplicationCore.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FptSenderService
    {
        private string _brandname;
        private string _accesstoken;
        private string _sessionId;
        public FptSenderService(string brandname, string accesstoken , string sessionId)
        {
            _brandname = brandname;
            _accesstoken = accesstoken;
            _sessionId = sessionId;
        }


        public async Task<FPTSendSMSResponseModel> SendSMS(string phone, string content)
        {


            ////url dev test IP : 14.169.99.3
            ///var url = "http://sandbox.sms.fpt.net/api/push-brandname-otp";
            if (string.IsNullOrEmpty(_accesstoken))
                return null;

            //url production IP: 14.169.99.3
            var url = "https://app.sms.fpt.net/api/push-brandname-otp";                   
          
            var data = new FPTSendSMSRequestModel
            {
                Phone = phone,
                access_token = _accesstoken,
                BrandName = _brandname,
                Message = content.EncodeBase64(),
                session_id = _sessionId
            };
            var client = new HttpClient();
            var jsonObj= JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(url, stringContent);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var sendResonse = JsonConvert.DeserializeObject<FPTSendSMSResponseModel>(response);

                return sendResonse;
            }

            return null;
        }      

    }

   

  


}
