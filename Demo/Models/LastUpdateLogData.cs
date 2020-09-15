using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Models
{
    public class LastUpdateLogData
    {
        public LastUpdateLogData()
        {
            Count = 0;
        }
        public int Count { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
