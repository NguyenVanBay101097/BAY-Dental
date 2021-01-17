using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CashBookSearch
    {
        public CashBookSearch()
        {
            Limit = 20;
            Offset = 0;
            Begin = false;
        }

        /// <summary>
        /// tinh dau ky hay k
        /// </summary>
        public bool Begin { get; set; }
        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// bank: ngan hang
        /// cash: tien mat
        /// cash_bank: tong quy
        /// </summary>
        public string ResultSelection { get; set; }
        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}
