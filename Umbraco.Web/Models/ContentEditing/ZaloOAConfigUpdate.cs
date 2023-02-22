using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ZaloOAConfigUpdate
    {
        /// <summary>
        /// Tự động gửi tin nhắn chúc mừng sinh nhật
        /// </summary>
        public bool AutoSendBirthdayMessage { get; set; }

        /// <summary>
        /// Nội dung tin nhắn chúc mừng sinh nhật
        /// </summary>
        public string BirthdayMessageContent { get; set; }
    }
}
