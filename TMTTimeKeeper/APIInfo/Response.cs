using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.APIInfo
{
   public class Response
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public ChamCongSync ModelError { get; set; }
        public string Message { get; set; }

    }
}
