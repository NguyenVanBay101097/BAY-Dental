using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Kich bản chăm sóc
    /// </summary>
    public class TCareCampaign: BaseEntity
    {
        public TCareCampaign()
        {
            State = "draft";
        }

        public string Name { get; set; }

        public string GraphXml { get; set; }

        public int SheduleStart { get; set; }

        /// <summary>
        /// draft : mới 
        /// running : đang chạy
        /// stopping : dừng
        /// </summary>
        public string State { get; set; }
        public string JobId { get; set; }

    }
}
