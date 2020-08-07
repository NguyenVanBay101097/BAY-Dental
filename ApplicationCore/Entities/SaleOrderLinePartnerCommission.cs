using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLinePartnerCommission : BaseEntity
    {
        /// <summary>
        /// bác sĩ
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// dịch vụ
        /// </summary>
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        /// <summary>
        /// bảng hoa hồng
        /// </summary>
        public Guid? CommissionId { get; set; }
        public Commission Commission { get; set; }

      
    }
}
