using System;
using System.Collections.Generic;
using System.Text;

namespace ZaloDotNetSDK.oa
{
    public class GetProfileOfFollowerResponse
    {
        public int error { get; set; }
        public string message { get; set; }
        public GetProfileOfFollowerResponseData data { get; set; }
    }

    public class GetProfileOfFollowerResponseData
    {
        public int user_gender { get; set; }
        public long user_id { get; set; }
        public long user_id_by_app { get; set; }
        public string avatar { get; set; }
        public string display_name { get; set; }
        public int birth_date { get; set; }
        public string shared_info { get; set; }
    }
}
