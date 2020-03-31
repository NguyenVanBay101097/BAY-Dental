using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookPage : BaseEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAccesstoken { get; set; }

        public string PageId { get; set; }

        public string PageName { get; set; }
        public string PageAccesstoken { get; set; }

        //Config Automations

        public Guid? AutoConfigId { get; set; }
        public FacebookScheduleAppointmentConfig AutoConfig { get; set; }
        //
    }
}
