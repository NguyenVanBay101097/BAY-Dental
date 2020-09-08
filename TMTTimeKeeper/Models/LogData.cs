using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class LogData
    {
        public int MachineNumber { get; set; }
        public int IndRegID { get; set; }
        public string EmployeeName { get; set; }
        public Guid EmployeeId { get; set; }
        public string DateTimeRecord { get; set; }

        public DateTime DateOnlyRecord
        {
            get { return DateTime.Parse(DateTime.Parse(DateTimeRecord).ToString("yyyy-MM-dd")); }
        }
        public DateTime TimeOnlyRecord
        {
            get { return DateTime.Parse(DateTime.Parse(DateTimeRecord).ToString("hh:mm:ss tt")); }
        }
        public string MyTimeOnlyRecord { get; set; }

        public int dwInOutMode { get; set; }
    }
}
