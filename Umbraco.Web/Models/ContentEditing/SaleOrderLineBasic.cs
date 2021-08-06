using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string State { get; set; }

        public Guid OrderPartnerId { get; set; }
        public PartnerSimple OrderPartner { get; set; }

        public Guid OrderId { get; set; }
        public SaleOrderBasic Order { get; set; }

        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public string Diagnostic { get; set; }

        public DateTime DateCreated { get; set; }

        public EmployeeBasic Employee { get; set; }

        public EmployeeBasic Assistant { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();

        public bool IsListLabo { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTotal { get; set; }

        /// <summary>
        /// Số tiền đã thanh toán
        /// </summary>
        public decimal? AmountPaid { get; set; }

        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manual :  thủ công
        /// </summary>
        public string ToothType { get; set; }

        public decimal? AmountResidual
        {
            get
            {
                return PriceTotal - AmountPaid;
            }
        }
    }

    public class SaleOrderLineSmsSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }
    }
}
