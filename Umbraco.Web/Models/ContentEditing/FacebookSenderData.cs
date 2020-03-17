using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class FacebookSender
    {
        [DeserializeAs(Name = "data")]
        public List<FacebookSenderData> Data { get; set; } = new List<FacebookSenderData>();
    }

    public class FacebookSenderData
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }
        [DeserializeAs(Name = "senders")]
        public Senders Senders { get; set; }

    }
    public class Senders {
        [DeserializeAs(Name = "data")]
        public List<FacebookSenderDataBasic> Data { get; set; } = new List<FacebookSenderDataBasic>();
    }
    public class FacebookSenderDataBasic
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }
       
        

    }
}
