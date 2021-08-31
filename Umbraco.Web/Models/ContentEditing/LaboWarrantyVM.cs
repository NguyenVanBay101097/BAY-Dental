using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboWarrantyPaged
    {
        public LaboWarrantyPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? LaboOrderId { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// sent: Đã gửi
        /// received: Đã nhận
        /// assembled: Đã lắp
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Ngày nhận bảo hành
        /// </summary>
        public DateTime? DateReceiptFrom { get; set; }
        public DateTime? DateReceiptTo { get; set; }
    }

    public class LaboWarrantySave
    {
        public LaboWarrantySave()
        {
            DateReceiptWarranty = DateTime.Now;
            State = "draft";
        }

        /// <summary>
        /// Phiếu bảo hành
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Phiếu Labo Id
        /// </summary>
        public Guid? LaboOrderId { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Ngày nhận bảo hành
        /// </summary>
        public DateTime? DateReceiptWarranty { get; set; }

        /// <summary>
        /// Danh sách răng
        /// </summary>
        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();

        /// <summary>
        /// Lý do bảo hành
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Nội dung bảo hành
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// new: Mới
        /// sent: Đã gửi
        /// received: Đã nhận
        /// assembled: Đã lắp
        /// </summary>
        public string State { get; set; }
    }

    public class LaboWarrantyGetDefault
    {
        public Guid? LaboOrderId { get; set; }
    }

    public class LaboWarrantySimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class LaboWarrantyBasic
    {
        public Guid Id { get; set; }


        /// <summary>
        /// Phiếu bảo hành
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Phiếu Labo
        /// </summary>
        public string LaboOrderId { get; set; }
        public string LaboOrderName { get; set; }

        /// <summary>
        /// Ngày nhận bảo hành
        /// </summary>
        public DateTime? DateReceiptWarranty { get; set; }

        /// <summary>
        /// Ngày gửi bảo hành
        /// </summary>
        public DateTime? DateSendWarranty { get; set; }

        /// <summary>
        /// Ngày nhận nghiệm thu
        /// </summary>
        public DateTime? DateReceiptInspection { get; set; }

        /// <summary>
        /// Ngày lắp bảo hành
        /// </summary>
        public DateTime? DateAssemblyWarranty { get; set; }

        /// <summary>
        /// Lý do bảo hành
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Nội dung bảo hành
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// new: Mới
        /// sent: Đã gửi
        /// received: Đã nhận
        /// assembled: Đã lắp
        /// </summary>
        public string State { get; set; }
    }

    public class LaboWarrantyDisplay
    {
        public LaboWarrantyDisplay()
        {
            DateReceiptWarranty = DateTime.Now;
            State = "draft";
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Phiếu bảo hành
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Phiếu Labo
        /// </summary>
        public string LaboOrderId { get; set; }
        public string LaboOrderName { get; set; }

        /// <summary>
        /// Nhà cung cấp Labo
        /// </summary>
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public string CustomerId { get; set; }
        public string CustomerRef { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDisplayName { get; set; }

        /// <summary>
        /// Loại phục hình
        /// </summary>
        public string SaleOrderLineName { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        /// <summary>
        /// Ngày nhận bảo hành
        /// </summary>
        public DateTime? DateReceiptWarranty { get; set; }

        /// <summary>
        /// Ngày gửi bảo hành
        /// </summary>
        public DateTime? DateSendWarranty { get; set; }

        /// <summary>
        /// Ngày nhận nghiệm thu
        /// </summary>
        public DateTime? DateReceiptInspection { get; set; }

        /// <summary>
        /// Ngày lắp bảo hành
        /// </summary>
        public DateTime? DateAssemblyWarranty { get; set; }

        /// <summary>
        /// Danh sách răng LaboOrder
        /// </summary>
        public IEnumerable<ToothDisplay> TeethLaboOrder { get; set; } = new List<ToothDisplay>();

        /// <summary>
        /// Danh sách răng
        /// </summary>
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();

        /// <summary>
        /// Lý do bảo hành
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Nội dung bảo hành
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// new: Mới
        /// sent: Đã gửi
        /// received: Đã nhận
        /// assembled: Đã lắp
        /// </summary>
        public string State { get; set; }
    }
}
