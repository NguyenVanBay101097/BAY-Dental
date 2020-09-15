using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.APIInfo
{
   public class Response
    {
        public bool Success { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
        public ChamCongSync ModelError { get; set; }
        public string Message { get; set; }

    }

    public class TimekeepingResponse
    {
        public IEnumerable<Response> ErrorDatas = new List<Response>();
    }
}
