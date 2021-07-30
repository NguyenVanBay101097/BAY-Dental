using ApplicationCore.Utilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
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
        private readonly IHttpClientFactory _clientFactory;
        private string _clientId;
        private string _clientSecret;

        public FptSenderService(IHttpClientFactory clientFactory, string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _clientFactory = clientFactory;
        }


        public async Task<FPTSendSMSResponseModel> SendSMS(string brandname, string access_token, string phone, string content)
        {

            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri("https://app.sms.fpt.net");

            var sessionId = StringUtils.RandomString(32);

            if (string.IsNullOrEmpty(access_token))
                return null;

            var data = new FPTSendSMSRequestModel
            {
                Phone = phone,
                access_token = access_token,
                BrandName = brandname,
                Message = content.EncodeBase64(),
                session_id = sessionId
            };

            var jsonObj = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("/api/push-brandname-otp", stringContent);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var sendResonse = JsonConvert.DeserializeObject<FPTSendSMSResponseModel>(response);

                return sendResonse;
            }

            return null;
        }


        public async Task<FPTAccessTokenResponseModel> GetApiToken(string clientId, string clientSecret)
        {
            var sessionId = StringUtils.RandomString(32);

            var data = new FPTAccessTokenRequestModel
            {
                client_id = clientId,
                client_secret = clientSecret,
                scope = "send_brandname_otp send_brandname",
                session_id = sessionId,
                grant_type = "client_credentials"
            };

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://app.sms.fpt.net");
            var jsonObject = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("/oauth2/token", stringContent);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<FPTAccessTokenResponseModel>(response);
                return tokenResponse;
            }

            return null;

        }


    }






}
