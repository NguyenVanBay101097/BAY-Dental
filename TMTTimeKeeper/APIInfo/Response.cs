using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.APIInfo
{
   public class Response
    {
        public string Message { get; set; }
        /// <summary>
        /// True: thành công
        /// False: không thành công
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Danh sách lỗi được add khi có lỗi
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

    }
}
