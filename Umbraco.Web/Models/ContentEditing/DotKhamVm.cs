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

        public int? Sequence { get; set; }

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

    /// <summary>
    /// Thong tin 1 dot kham
    /// </summary>
    public class DotKhamDisplayVm
    {
        public Guid Id { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public EmployeeSimple Doctor { get; set; }

        public IEnumerable<DotKhamLineDisplay> Lines { get; set; } = new List<DotKhamLineDisplay>();

        /// <summary>
        /// hình ảnh
        /// </summary>
        public IEnumerable<PartnerImageDisplay> DotKhamImages { get; set; } = new List<PartnerImageDisplay>();

        public string Name { get; set; }

    }


    /// <summary>
    /// class dung de save dot kham
    /// </summary>
    public class DotKhamSaveVm
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }

        public IEnumerable<DotKhamLineSaveVM> Lines { get; set; } = new List<DotKhamLineSaveVM>();

        /// <summary>
        /// hình ảnh
        /// </summary>
        public IEnumerable<PartnerImageSave> DotKhamImages { get; set; } = new List<PartnerImageSave>();
    }

    public class GetAllDotKhamVm
    {
        public ICollection<Guid> OrderIds { get; set; } = new List<Guid>(); 
    }
}
