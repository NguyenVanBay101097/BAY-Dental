using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ZaloOAConfig: BaseEntity
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string Avatar { get; set; }

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
