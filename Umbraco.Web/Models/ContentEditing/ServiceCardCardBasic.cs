using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardCardBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }

        public string CardTypeName { get; set; }

        public string PartnerName { get; set; }

        public DateTime? ActivatedDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        /// <summary>
        /// Số tiền trong thẻ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Số tiền còn lại
        /// </summary>
        public decimal? Residual { get; set; }

        public string State { get; set; }
    }
}
