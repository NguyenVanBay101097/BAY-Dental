using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LongTermAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
