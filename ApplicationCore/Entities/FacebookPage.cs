using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Kênh gửi
    /// </summary>
    public class FacebookPage : BaseEntity
    {
        /// <summary>
        /// facebook: Facebook
        /// zalo: Zalo
        /// </summary>
        public string Type { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAccesstoken { get; set; }

        public string PageId { get; set; }

        public string PageName { get; set; }

        public string PageAccesstoken { get; set; }

        public string Avatar { get; set; }

        //Config Automations

        public Guid? AutoConfigId { get; set; }
        public FacebookScheduleAppointmentConfig AutoConfig { get; set; }
        //
    }
}
