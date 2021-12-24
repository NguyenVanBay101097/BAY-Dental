using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleMakeProductRequestVM
    {
        /// <summary>
        /// người yêu cầu
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// ngày yêu cầu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// bác sĩ chỉ định
        /// </summary>
        public Guid EmployeeId { get; set; }

        public Guid SaleOrderId { get; set; }

        public IEnumerable<SaleMakeProductRequestVMLine> Lines { get; set; } = new List<SaleMakeProductRequestVMLine>();

    }

    public class SaleMakeProductRequestVMLine
    {
        public decimal ProductQty { get; set; }

        public Guid SaleProductionLineId { get; set; }
    }
}
