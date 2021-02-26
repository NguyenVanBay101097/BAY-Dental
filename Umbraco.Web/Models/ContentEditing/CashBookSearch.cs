using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CashBookSearch
    {
        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// bank: ngan hang
        /// cash: tien mat
        /// cash_bank: tong quy
        /// </summary>
        public string ResultSelection { get; set; }
    }
}
