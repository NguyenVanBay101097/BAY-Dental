using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductRequest : BaseEntity
    {
        public ProductRequest()
        {
            State = "draft";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// người yêu cầu
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        /// <summary>
        /// ngày yêu cầu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// bác sĩ chỉ định
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// phiếu xuất
        /// </summary>
        public Guid? PickingId { get; set; }
        public StockPicking Picking { get; set; }

        public ICollection<ProductRequestLine> Lines { get; set; } = new List<ProductRequestLine>();

        /// <summary>
        /// draft : nháp
        /// confirmed : đang yêu cầu
        /// done : đã xuất
        /// </summary>
        public string State { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        /// <summary>
        /// related: SaleOrder.CompanyId
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
