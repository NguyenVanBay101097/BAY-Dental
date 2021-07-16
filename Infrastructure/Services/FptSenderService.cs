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
        private static readonly Object _lock = new Object();
        private readonly IDistributedCache _cache;
        private const int cacheExpirationInDays = 1;
        private readonly IHttpClientFactory _clientFactory;
        private string _clientId;
        private string _clientSecret;

        public FptSenderService(IHttpClientFactory clientFactory, IDistributedCache cache, string clientId , string clientSecret)
        {
            _clientId = clientId;
            _cache = cache;
            _clientSecret = clientSecret;
            _clientFactory = clientFactory;
        }


        public async Task<FPTSendSMSResponseModel> SendSMS(string brandname, string phone, string content)
        {

            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri("https://app.sms.fpt.net");

            var sessionId = StringUtils.RandomString(32);

            var access_token = await GetApiToken(_clientId, _clientSecret, sessionId);

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

        public async Task<string> GetApiToken(string clientId, string clientsecret, string sessionId)
        {
            var accessToken = GetFromCache(clientId);

            if (accessToken != null)
            {
                if (accessToken.ExpiresIn > DateTime.Now)
                {
                    return accessToken.AccessToken;
                }            
            }

            // add
            var newAccessToken = await getApiToken(clientId, clientsecret, sessionId);
            AddToCache(clientId, newAccessToken);

            return newAccessToken.AccessToken;
        }


        private async Task<AccessTokenItem> getApiToken(string clientId, string clientsecret, string sessionId)
        {
            try
            {
                var data = new FPTAccessTokenRequestModel
                {
                    client_id = clientId,
                    client_secret = clientsecret,
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

                    return new AccessTokenItem
                    {
                        ExpiresIn = DateTime.Now.AddSeconds(tokenResponse.expires_in.Value),
                        AccessToken = tokenResponse.access_token
                    };
                }

                return null;

            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }

        private void AddToCache(string key, AccessTokenItem accessTokenItem)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(cacheExpirationInDays));

            lock (_lock)
            {
                _cache.SetString(key, JsonConvert.SerializeObject(accessTokenItem), options);
            }
        }

        private AccessTokenItem GetFromCache(string key)
        {
            var item = _cache.GetString(key);
            if (item != null)
            {
                return JsonConvert.DeserializeObject<AccessTokenItem>(item);
            }

            return null;
        }



        private class AccessTokenItem
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresIn { get; set; }
        }

    }






}
