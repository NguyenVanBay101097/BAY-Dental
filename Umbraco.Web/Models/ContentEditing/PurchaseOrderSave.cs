using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderSave
    {

        public string Type { get; set; }
        public Guid PickingTypeId { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid PartnerId { get; set; }

        public Guid? JournalId { get; set; }

        public DateTime DateOrder { get; set; }

        public decimal? AmountPayment { get; set; }

        public IEnumerable<PurchaseOrderLineSave> OrderLines { get; set; } = new List<PurchaseOrderLineSave>();

        public string Notes { get; set; }
    }
}
