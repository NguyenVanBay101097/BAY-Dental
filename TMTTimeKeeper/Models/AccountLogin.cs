using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class AccountLogin
    {
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("AccessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("RefeshToken")]
        public string RefeshToken { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("CompanyId")]
        public string CompanyId { get; set; }

        [JsonProperty("CompanyName")]
        public string CompanyName { get; set; }
    }
}
