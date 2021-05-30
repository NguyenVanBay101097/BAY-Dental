using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AppointmentSumaryCountVM
    {
        public int TotalConfirmed { get; set; }
        public int TotalCancel { get; set; }
        public int TotalWating { get; set; }

    }
}
