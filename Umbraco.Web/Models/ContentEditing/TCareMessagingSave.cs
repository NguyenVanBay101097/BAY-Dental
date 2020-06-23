using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareMessagingSave
    {
        public TCareMessagingSave()
        {
            MethodType = "interval";
            IntervalType = "minutes";
            IntervalNumber = 0;
            Content = "";
        }


        /// <summary>
        /// phương thức :
        /// interval : trước thời gian
        /// shedule : lên lịch ngày giờ cụ thể
        /// </summary>
        public string MethodType { get; set; }

        /// <summary>
        /// MethodType : interval
        /// "minutes" , "hours" , "weeks", "months"
        /// </summary>
        public string IntervalType { get; set; }

        public int? IntervalNumber { get; set; }

        /// <summary>
        /// MethodType : shedule
        /// </summary>
        public DateTime? SheduleDate { get; set; }

        public string Content { get; set; }

        //--Kenh gui ---//

        /// <summary>
        ///  priority : ưu tiên
        ///  fixed : cố định
        /// </summary>
        public string ChannelType { get; set; }

        public Guid? ChannelSocialId { get; set; }
        public Guid TCareCampaignId { get; set; }
        
    }
}
