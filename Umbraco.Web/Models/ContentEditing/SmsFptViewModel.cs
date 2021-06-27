using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FPTAccessTokenRequestModel
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }

        /// <summary>
        /// send_brandname_otp send_brandname : quyên gửi tin và tin nhắn quảng cáo
        /// </summary>
        public string scope { get; set; }
        public string session_id { get; set; }
    }

    public class FPTAccessTokenResponseModel
    {
        public string access_token { get; set; }
        public int? expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public int? error { get; set; }
        public string error_description { get; set; }
    }

        public class FPTSendSMSRequestModel
    {
        /// <summary>
        /// Access token 
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Session id lúc đăng ký 
        /// access token
        /// </summary>
        public string session_id { get; set; }

        /// <summary>
        /// Brandname đã đăng 
        /// ký với FPT
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// Số điện thoại nhận tin 
        /// nhắn.Định dạng 
        /// 84xxx, 0xxx.
        /// Ví dụ: 84949123456 
        /// hoặc 0949123456
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Nội dung tin nhắn gửi đi.
        /// Lưu ý: nội dung phải
        /// được mã hóa base64
        /// </summary>
        public string Message { get; set; }

        public string RequestId { get; set; }
    }

    public class FPTSendSMSResponseModel
    {
        /// <summary>
        /// Id của tin brandname gửi đi. 
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Tên brandname 
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// Số điện thoại gửi tin nhắn. Định dạng 84xxxx
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Nội dung tin nhắn gửi đi
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ID của đối tác
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Nhà mạng của thuê bao khách hàng.
        /// </summary>
        public string Telco { get; set; }

        public int? Error { get; set; }

        public string Error_description { get; set; }
    }
}
