using System;
using System.Collections.Generic;
using System.Text;

namespace ZaloDotNetSDK.oa
{
    public class GetProfileOAResponse
    {
        public int error { get; set; }
        public string message { get; set; }
        public GetProfileOAData data { get; set; }
    }

    public class GetProfileOAData
    {
        public long oa_id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string cover { get; set; }
    }
}
