using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardTypeSave
    {
        public string Name { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Số tiền trong thẻ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Thời hạn
        /// month: Tháng
        /// </summary>
        public string Period { get; set; }

        public int? NbrPeriod { get; set; }
    }
}
