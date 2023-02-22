using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class ReadLogResult
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public IEnumerable<ReadLogResultData> Data { get; set; } = new List<ReadLogResultData>();
    }

    public class ReadLogResultData
    {
        public string UserId { get; set; }

        public string VerifyDate { get; set; }

        public int VerifyType { get; set; }

        public int VerifyState { get; set; }

        public int WorkCode { get; set; }
    }
}
