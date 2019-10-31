using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboOrder: BaseEntity
    {
        public LaboOrder()
        {
            DateOrder = DateTime.Now;
            State = "draft";
        }

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
        public Partner Partner { get; set; }

        public Guid? CustomerId { get; set; }
        public Partner Customer { get; set; }

        public DateTime DateOrder { get; set; }

        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Ngày nhận (dự kiến)
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<LaboOrderLine> OrderLines { get; set; } = new List<LaboOrderLine>();

        public Guid? DotKhamId { get; set; }
        public DotKham DotKham { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
