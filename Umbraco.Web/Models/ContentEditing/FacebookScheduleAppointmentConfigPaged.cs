using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookScheduleAppointmentConfigPaged
    {
        public FacebookScheduleAppointmentConfigPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public bool AutoShedule { get; set; }
        public string Search { get; set; }
    }
}
