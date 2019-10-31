using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderLineBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }
        public PartnerSimple Customer { get; set; }

        /// <summary>
        /// Tên labo
        /// </summary>
        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid SupplierId { get; set; }
        public PartnerSimple Supplier { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Ngày gửi
        /// </summary>
        public DateTime? SentDate { get; set; }

        /// <summary>
        /// Ngày nhận
        /// </summary>
        public DateTime? ReceivedDate { get; set; }
    }

   
    public class LaboOrderLinePaged
    {
        public LaboOrderLinePaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? SupplierId { get; set; }

        public Guid? ProductId { get; set; }

        public string SearchCustomer { get; set; }

        public string SearchSupplier { get; set; }

        public string SearchProduct { get; set; }

        public DateTime? SentDateFrom { get; set; }

        public DateTime? SentDateTo { get; set; }

        public DateTime? ReceivedDateFrom { get; set; }

        public DateTime? ReceivedDateTo { get; set; }

        public string Search { get; set; }
    }

    public class LaboOrderLineDefaultGet
    {
        public Guid? InvoiceId { get; set; }
        public Guid? DotKhamId { get; set; }
    }

    public class LaboOrderLineOnChangeProduct
    {
        public Guid? ProductId { get; set; }
    }

    public class LaboOrderLineOnChangeProductResult
    {
        public decimal PriceUnit { get; set; }
    }
}
