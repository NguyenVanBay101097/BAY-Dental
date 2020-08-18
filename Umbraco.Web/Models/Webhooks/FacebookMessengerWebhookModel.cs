using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.Webhooks
{
    public class FacebookWebHook
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("entry")]
        public FacebookWebHookEntry[] Entry { get; set; }
    }

    public class FacebookWebHookEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("time")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("messaging")]
        public FacebookWebHookEntryMessaging[] Messaging { get; set; }
    }

    public class FacebookWebHookEntryMessaging
    {
        [JsonProperty("sender")]
        public FacebookWebHookEntryMessagingSender Sender { get; set; }

        [JsonProperty("recipient")]
        public FacebookWebHookEntryMessagingSender Recipient { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("read")]
        public FacebookWebHookEntryMessagingRead Read { get; set; }

        [JsonProperty("delivery")]
        public FacebookWebHookEntryMessagingDelivery Delivery { get; set; }

        [JsonProperty("message")]
        public FacebookWebHookEntryMessagingMessage Message { get; set; }
    }

    public class FacebookWebHookEntryMessagingSender
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class FacebookWebHookEntryMessagingRead
    {
        [JsonProperty("watermark")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Watermark { get; set; }
    }

    public class FacebookWebHookEntryMessagingDelivery
    {
        [JsonProperty("mids")]
        public IEnumerable<string> Mids { get; set; }

        [JsonProperty("watermark")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Watermark { get; set; }
    }

    public class FacebookWebHookEntryMessagingMessage
    {
        [JsonProperty("mid")]
        public string Source { get; set; }

        [JsonProperty("text")]
        public string Type { get; set; }

        [JsonProperty("quick_reply")]
        public MessageQuickReply QuickReply { get; set; }

        //[JsonProperty("referer_uri")]
        //public string RefererUri { get; set; }
    }

    public class MessageQuickReply
    {
        [JsonProperty("payload")]
        public string Payload { get; set; }
    }




    public class UnixTimestampConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalSeconds.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            double d = double.Parse(reader.Value.ToString());
            return _epoch.AddMilliseconds(d);
        }
    }
}
