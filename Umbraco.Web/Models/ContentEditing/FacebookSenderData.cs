using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class Conversations
    {
        [DeserializeAs(Name = "data")]
        public List<FacebookSenders> Data { get; set; } = new List<FacebookSenders>();
        [DeserializeAs(Name = "paging")]
        public ConversationsPaged paging { get; set; }
    }

    public class FacebookSenders
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }
        [DeserializeAs(Name = "senders")]
        public FacebookSenderData Senders { get; set; }

    }
    public class FacebookSenderData
    {
        [DeserializeAs(Name = "data")]
        public List<FacebookSenderDataBasic> Data { get; set; } = new List<FacebookSenderDataBasic>();
    }
    public class FacebookSenderDataBasic
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }
       
        

    }

    public class ConversationsPaged
    {
        [DeserializeAs(Name = "next")]
        public string PageNext { get; set; }
    }
}
