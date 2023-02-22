using System;
using System.Collections.Generic;
using System.Text;

namespace ZaloDotNetSDK.oa
{
    public class GetFollowersResponse
    {
        public int error { get; set; }
        public string message { get; set; }
        public GetFollowersData data { get; set; }
    }

    public class GetFollowersData
    {
        public int total { get; set; }
        public IEnumerable<GetFollowersDataFollowers> followers { get; set; }
    }

    public class GetFollowersDataFollowers
    {
        public string user_id { get; set; }
    }


    public class GetMessageUserFollowersResponse
    {
        public int error { get; set; }
        public string message { get; set; }
        public IEnumerable<GetMessageFollowersData> data { get; set; } = new List<GetMessageFollowersData>();
    }

    public class GetMessageFollowersData
    {
        public int src { get; set; }
        //public int time { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public string message_id { get; set; }
        public string thumb { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string from_id { get; set; }
        public string to_id { get; set; }
        public string from_display_name { get; set; }
        public string from_avatar { get; set; }
        public string to_display_name { get; set; }
        public string to_avatar { get; set; }
        public string location { get; set; }
    }
}
