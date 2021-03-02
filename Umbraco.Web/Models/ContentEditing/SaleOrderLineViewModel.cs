using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public Guid OrderId { get; set; }
        public SaleOrderBasic Order { get; set; }
        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }
        public string Diagnostic { get; set; }
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
        public IEnumerable<DotKhamStepBasic> Steps { get; set; } = new List<DotKhamStepBasic>();
    }

    public class SaleOrderLineSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class SaleOrderLineSaleTodayVM
    {
        public Guid Id { get; set; }

        public string OrderName { get; set; }

        public string PartnerDisplayName { get; set; }

        public string EmployeeName { get; set; }

        public string ProductName { get; set; }

        public decimal PriceTotal { get; set; }

        /// <summary>
        /// Số tiền đã thanh toán
        /// </summary>
        public decimal? AmountPaid { get; set; }

        /// <summary>
        /// Tiền còn nợ
        /// </summary>
        public decimal? AmountResidual { get; set; }

        public string State { get; set; }
    }

    public class SaleOrderLineForProductRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProductBomForSaleOrderLine> Boms { get; set; } = new List<ProductBomForSaleOrderLine>();
    }
}
