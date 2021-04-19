using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Advisory : BaseEntity
    {
        public Advisory()
        {
            Date = DateTime.Now;
        }
        /// <summary>
        /// Ngày tư vấn
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// người tư vấn
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        /// <summary>
        /// khách hàng
        /// </summary>
        public Guid? CustomerId { get; set; }
        public Partner Customer { get; set; }
        public Guid? ToothCategoryId { get; set; }
        public ToothCategory ToothCategory { get; set; }
        public ICollection<AdvisoryToothRel> AdvisoryToothRels { get; set; } = new List<AdvisoryToothRel>();
        public ICollection<AdvisoryToothDiagnosisRel> AdvisoryToothDiagnosisRels { get; set; } = new List<AdvisoryToothDiagnosisRel>();
        public ICollection<AdvisoryProductRel> AdvisoryProductRels { get; set; } = new List<AdvisoryProductRel>();
        public ICollection<QuotationLine> QuotationLines { get; set; } = new List<QuotationLine>();
        public ICollection<SaleOrderLine> OrderLines { get; set; } = new List<SaleOrderLine>();
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
