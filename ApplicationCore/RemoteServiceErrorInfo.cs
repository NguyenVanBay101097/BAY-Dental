using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    /// <summary>
    /// Used to store information about an error.
    /// </summary>
    public class RemoteServiceErrorInfo
    {
        public string Message { get; set; }

        /// <summary>
        /// Xác định loại lỗi ví dụ: ValidationError
        /// </summary>
        public string Name { get; set; }
    }
}
