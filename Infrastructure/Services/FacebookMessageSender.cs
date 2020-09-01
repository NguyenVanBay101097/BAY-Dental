using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookMessageSender : IFacebookMessageSender
    {
        public async Task<SendFacebookMessageReponse> SendMessageTagTextAsync(string text, string psid, string access_token, string tag = "ACCOUNT_UPDATE")
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "MESSAGE_TAG");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = text }));
            request.AddParameter("tag", tag);

            try
            {
                var response = await request.ExecuteAsync<SendFacebookMessageReponse>();
                if (response.GetExceptions().Any())
                {
                    return null;
                }
                else
                {
                    var result = response.GetResult();
                    return result;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<SendFacebookMessageReponse> SendMessageMarketingTextAsync(object message, string psid, string access_token, string tag = "ACCOUNT_UPDATE")
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "MESSAGE_TAG");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));
            request.AddParameter("tag", tag);

            try
            {
                var response = await request.ExecuteAsync<SendFacebookMessageReponse>();
                if (response.GetExceptions().Any())
                {
                    return null;
                }
                else
                {
                    var result = response.GetResult();
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<SendFacebookMessageReponse> SendMessageTCareTextAsync(string message, string psid, string access_token, string tag = "ACCOUNT_UPDATE")
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "MESSAGE_TAG");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));
            request.AddParameter("tag", tag);

            var response = await request.ExecuteAsync<SendFacebookMessageReponse>();
            if (response.GetExceptions().Any())
            {
                return new SendFacebookMessageReponse 
                { 
                    error = string.Join(";", response.GetExceptions().Select(x => x.Message))
                };
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }
    }

    public class SendFacebookMessageReponse
    {
        public string message_id { get; set; }
        public string recipient_id { get; set; }
        public string error { get; set; }
    }

    public class FacebookMessageApiInfoReponse
    {
        public string id { get; set; }
        public DateTime created_time { get; set; }
        public string message { get; set; }
    }
}
