using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class RemoteServiceErrorResponse
    {
        /// <summary>
        /// Tiêu đề ví dụ: Odoo Server Error, Odoo Session Invalid
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int Code { get; set; }

        public RemoteServiceErrorInfo Data { get; set; }
    }
}
