using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LoaiThuChiBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

      
        public bool IsInclude { get; set; }

        /// <summary>
        /// Hiện trong hạch toán két quả kinh doanh
        /// </summary>
        public bool IsAccounting { get; set; }
    }
}
