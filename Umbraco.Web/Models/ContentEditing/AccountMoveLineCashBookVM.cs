using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountMoveLineCashBookVM
    {
        /// <summary>
        /// Ngay
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// ma
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// description
        /// </summary>
        public string Ref { get; set; }
        /// <summary>
        /// chi
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// thu
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// Kh
        /// </summary>
        public string PartnerName { get; set; }

        public string JournalType { get; set; }
        /// <summary>
        /// chi nhanh
        /// </summary>
        public Guid CompanyId { get; set; }

    }
}
