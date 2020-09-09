using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.APIInfo
{
    public class ChamCongSave
    {
        public Guid EmployeeId { get; set; }

        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public Guid WorkEntryTypeId { get; set; }

        public Guid CompanyId { get; set; }
    }
}
