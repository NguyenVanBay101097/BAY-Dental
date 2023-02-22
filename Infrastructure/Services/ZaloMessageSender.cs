using System;
using System.Collections.Generic;
using System.Text;
using ZaloDotNetSDK;

namespace Infrastructure.Services
{
    public class ZaloMessageSender
    {
        public SendMessageZaloResponse SendText(string message, string access_token, string uid)
        {
            var zaloClient = new ZaloClient(access_token);
            var sendResult = zaloClient.sendTextMessageToUserId(uid, message).ToObject<SendMessageZaloResponse>();
            return sendResult;
        }
    }
}
