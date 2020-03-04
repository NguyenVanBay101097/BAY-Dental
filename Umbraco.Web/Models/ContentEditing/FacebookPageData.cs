using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   
        public class FacebookPageData
        {
            public List<FacebookPageBasic> Data { get; set; }
        }
        public class FacebookPageBasic
        {
            public long Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("access_token")]
            public string PageAccesstoken { get; set; }
            //public FacebookPictureData Picture { get; set; }
            //public FacebookUserAccessTokenData DataPages { get; set; }
        }
    
}
