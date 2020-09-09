using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
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
 
        public IEnumerable<Employee> Items { get; set; }
    }
}
