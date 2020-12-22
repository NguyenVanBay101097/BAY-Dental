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
  
        public string CustomerName { get; set; }

        public string ProductName { get; set; }

        public string PartnerName { get; set; }

        public decimal ProductQty { get; set; }

        public decimal PriceTotal { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        public DateTime DateOrder { get; set; }

        public DateTime? DatePlanned { get; set; }

        public string State { get; set; }

        public string OrderName { get; set; }

        public bool IsReceived { get; set; }
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

        public string Search { get; set; }
    }

    public class LaboOrderLineDefaultGet
    {
        public Guid? SaleOrderLineId { get; set; }      
    }

    public class LaboOrderLineOnChangeProduct
    {
        public Guid? ProductId { get; set; }
    }

    public class LaboOrderLineOnChangeProductResult
    {
        public string Name { get; set; }
        public decimal PriceUnit { get; set; }
    }
}
