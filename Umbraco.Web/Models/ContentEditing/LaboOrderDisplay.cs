using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderDisplay
    {
        public LaboOrderDisplay()
        {
            DateOrder = DateTime.Now;
            State = "draft";
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        /// <summary>
        /// Vendor Reference
        /// </summary>
        public string PartnerRef { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public DateTime DateOrder { get; set; }

        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Ngày nhận (dự kiến)
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        public IEnumerable<LaboOrderLineDisplay> OrderLines { get; set; } = new List<LaboOrderLineDisplay>();

        public Guid? DotKhamId { get; set; }
        public DotKhamSimple DotKham { get; set; }

        public Guid? CustomerId { get; set; }
        public PartnerSimple Customer { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrderBasic SaleOrder { get; set; }
    }
}
