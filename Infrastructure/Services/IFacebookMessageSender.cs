using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IFacebookMessageSender
    {
        Task<SendFacebookMessageReponse> SendMessageTagTextAsync(string text, string psid, string access_token, string tag = "ACCOUNT_UPDATE");
        Task<SendFacebookMessageReponse> SendMessageMarketingTextAsync(object message, string psid, string access_token, string tag = "ACCOUNT_UPDATE");
    }
}
