using Newtonsoft.Json;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class FacebookPageData
    {
        [DeserializeAs(Name = "data")]
        public List<FacebookPageDataBasic> Data { get; set; } = new List<FacebookPageDataBasic>();
        
    }
    public class FacebookPageDataBasic
    {
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }
        [DeserializeAs(Name = "name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "access_token")]
        public string PageAccesstoken { get; set; }
        
    }

}
