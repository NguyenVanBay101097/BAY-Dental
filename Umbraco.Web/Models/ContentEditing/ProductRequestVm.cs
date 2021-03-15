using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductRequestBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// ngày yêu cầu
        /// </summary>
        public DateTime Date { get; set; }

        public string EmployeeName { get; set; }

        public string PickingName { get; set; }
        public StockPickingBasic Picking { get; set; }

        /// <summary>
        /// draft : nháp
        /// confirmed : đang yêu cầu
        /// done : đã xuất
        /// </summary>
        public string State { get; set; }
    }

    public class ProductRequestPaged
    {
        public ProductRequestPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public Guid? SaleOrderId { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public string State { get; set; }

    }


    public class ProductRequestDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// người yêu cầu
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        /// <summary>
        /// ngày yêu cầu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// bác sĩ chỉ định
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        public Guid? SaleOrderId { get; set; }

        /// <summary>
        /// phiếu xuất
        /// </summary>
        public Guid? PickingId { get; set; }
        public StockPickingSimple Picking { get; set; }

        public IEnumerable<ProductRequestLineDisplay> Lines { get; set; } = new List<ProductRequestLineDisplay>();

        /// <summary>
        /// draft : nháp
        /// confirmed : đang yêu cầu
        /// done : đã xuất
        /// </summary>
        public string State { get; set; }
    }

    public class ProductRequestSave
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

        public IEnumerable<ProductRequestLineSave> Lines { get; set; } = new List<ProductRequestLineSave>();
    }

    public class ProductRequestGetLinePar
    {
        public Guid SaleOrderLineId { get; set; }
        public Guid ProductBomId { get; set; }
    }
}
