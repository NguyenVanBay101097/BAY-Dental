using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.Webhooks
{
    public class FacebookMessengerReadsWebhook
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("entry")]
        public FacebookMessengerReadsWebhookEntry[] Entry { get; set; }
    }

    public class FacebookMessengerReadsWebhookEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("time")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("messaging")]
        public FacebookMessengerReadsWebhookEntryMessaging[] Messaging { get; set; }
    }

    public class FacebookMessengerReadsWebhookEntryMessaging
    {
        [JsonProperty("sender")]
        public FacebookMessengerReadsWebhookEntryMessagingSender Sender { get; set; }

        [JsonProperty("recipient")]
        public FacebookMessengerReadsWebhookEntryMessagingRecipient Recipient { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("read")]
        public FacebookMessengerReadsWebhookEntryMessagingRead Read { get; set; }
    }

    public class FacebookMessengerReadsWebhookEntryMessagingSender
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class FacebookMessengerReadsWebhookEntryMessagingRead
    {
        [JsonProperty("watermark")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Watermark { get; set; }
    }

    public class FacebookMessengerReadsWebhookEntryMessagingRecipient
    {
        [JsonProperty("id")]
        public string Id { get; set; }
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
            return _epoch.AddSeconds((long)reader.Value);
        }
    }
}
