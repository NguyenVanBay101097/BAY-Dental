using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamVm
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã đợt khám
        /// </summary>
        public string Name { get; set; }

        public Guid? SaleOrderId { get; set; }

        /// <summary>
        /// Khách hàng lấy từ SaleOrder
        /// </summary>
        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        public DateTime DateCreated { get; set; }
        

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// confirmed: Đã xác nhận
        /// cancel: Hủy bỏ
        /// </summary>
        public string State { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }

        public Guid? AppointmentId { get; set; }

        public ICollection<DotKhamLineDisplay> Lines { get; set; } = new List<DotKhamLineDisplay>();


       

        /// <summary>
        /// hình ảnh
        /// </summary>
        public ICollection<PartnerImageBasic> DotKhamImages { get; set; } = new List<PartnerImageBasic>();
     
    }

    public class DotKhamSaveVm
    {

        /// <summary>
        /// Mã đợt khám
        /// </summary>
        public string Name { get; set; }

        public Guid? SaleOrderId { get; set; }

        /// <summary>
        /// Khách hàng lấy từ SaleOrder
        /// </summary>
        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }



        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// confirmed: Đã xác nhận
        /// cancel: Hủy bỏ
        /// </summary>
        public string State { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }

        public Guid? AppointmentId { get; set; }

        public ICollection<DotKhamLineSave> Lines { get; set; } = new List<DotKhamLineSave>();

        /// <summary>
        /// hình ảnh
        /// </summary>
        public ICollection<PartnerImageBasic> DotKhamImages { get; set; } = new List<PartnerImageBasic>();

    }

    public class GetAllDotKhamVm
    {
        public ICollection<Guid> OrderIds { get; set; } = new List<Guid>(); 
    }
}
