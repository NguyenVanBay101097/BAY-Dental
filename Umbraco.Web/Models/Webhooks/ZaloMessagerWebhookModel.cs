using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.Webhooks
{

    public class ZaloWebHook
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("sender")]
        public ZaloSender Sender { get; set; }

        [JsonProperty("recipient")]
        public ZaloRecipient Recipient { get; set; }

        /// <summary>
        /// Tên sự kiện : 
        /// user_received_message: người dùng nhận tin nhắn
        /// user_seen_message : người dùng đã xem tin nhắn
        /// </summary>
        [JsonProperty("event_name")]
        public string EventName { get; set; }

        [JsonProperty("message")]
        public ZaloMessage Message { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Timestamp { get; set; }
    }


    public class ZaloSender
    {
        //Id của Official Account gửi tin nhắn
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ZaloRecipient
    {
        //Id của User nhận tin nhắn
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ZaloMessage
    {
        [JsonProperty("msg_ids")]
        public IEnumerable<string> ids { get; set; }
    }


}
