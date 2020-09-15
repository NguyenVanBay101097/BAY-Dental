using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Models
{
   public class EmployeePagging
    {
        public EmployeePagging(long totalItems, long offset, long limit)
        {
            TotalItems = totalItems;
            Offset = offset;
            Limit = limit;
        }

        public long Offset { get; private set; }
   
        public long Limit { get; private set; }
  
        public long TotalItems { get; private set; }
 
        public IEnumerable<EmployeeSync> Items { get; set; }
    }
}
