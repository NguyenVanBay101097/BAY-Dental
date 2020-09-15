using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.APIInfo
{
    public class ChamCongSync
    {
        public Guid? EmpId { get; set; }
        public string IdMayChamCong { get; set; }
        public DateTime? Date { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public Guid? WorkId { get; set; }
    }
}
