using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiPaged
    {
        public PhieuThuChiPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        /// <summary>
        /// thu
        /// chi
        /// </summary>
        public string Type { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public Guid? PartnerId { get; set; }

        /// <summary>
        /// commission
        /// customer_debt
        /// other
        /// </summary>
        public string AccountType { get; set; }
    }
}
