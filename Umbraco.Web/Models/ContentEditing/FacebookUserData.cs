using Newtonsoft.Json;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
       
        [JsonProperty("name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "accounts")]
        public FacebookPageData Data { get; set; } 
    }
}
