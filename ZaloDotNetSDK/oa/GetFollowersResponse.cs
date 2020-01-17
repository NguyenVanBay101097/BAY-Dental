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
}
