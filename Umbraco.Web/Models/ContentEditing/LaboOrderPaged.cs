using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderPaged
    {
        public LaboOrderPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public Guid? PartnerId { get; set; }
        public DateTime? DateOrderFrom { get; set; }
        public DateTime? DateOrderTo { get; set; }
        public DateTime? DatePlannedFrom { get; set; }
        public DateTime? DatePlannedTo { get; set; }
        public string State { get; set; }
        public Guid? SaleOrderId { get; set; }
        public Guid? SaleOrderLineId { get; set; }
        public Guid? CustomerId { get; set; }
    }

    public class OrderLaboPaged
    {
        public OrderLaboPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        /// <summary>
        /// trể hạn
        /// Chờ nhận
        /// tới hạn
        /// </summary>
        public string State { get; set; }
    }

    public class ExportLaboPaged
    {
        public ExportLaboPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        public DateTime? DateExportFrom { get; set; }

        public DateTime? DateExportTo { get; set; }

        /// <summary>
        /// danhan
        /// chuanhan
        /// </summary>
        public string State { get; set; }
    }



    public class LaboOrderStatisticsPaged
    {
        public LaboOrderStatisticsPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public Guid? PartnerId { get; set; }

        public DateTime? DateOrderFrom { get; set; }

        public DateTime? DateOrderTo { get; set; }

        public DateTime? DatePlannedFrom { get; set; }

        public DateTime? DatePlannedTo { get; set; }

        public Guid? ProductId { get; set; }
    }

    public class LaboOrderStatisticsBasic
    {
        public Guid Id { get; set; }

        public decimal ProductQty { get; set; }

        public string ProductName { get; set; }

        public string PartnerDisplayName { get; set; }

        public string CustomerDisplayName { get; set; }

        public decimal PriceTotal { get; set; }

        public DateTime OrderDateOrder { get; set; }

        public DateTime? OrderDatePlanned { get; set; }

        public string OrderName { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        public string Note { get; set; }

        public string State { get; set; }

        public string SaleOrderName { get; set; }

        public Guid? SaleOrderId { get; set; }

        public bool IsReceived { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }

    public class LaboOrderGetCount
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public string State { get; set; }
    }

    public class LaboOrderReportOutput
    {
        public int LaboReceived { get; set; }
        public int LaboAppointment { get; set; }
    }
}
