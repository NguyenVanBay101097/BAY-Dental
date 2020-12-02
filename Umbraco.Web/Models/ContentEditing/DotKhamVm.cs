using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamVm
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Ma tien trinh
        /// </summary>
        public string Name { get; set; }


        public Guid? SaleOrderId { get; set; }
        public SaleOrderBasic SaleOrder { get; set; }

        /// <summary>
        /// Khách hàng lấy từ invoice
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
        public CompanySimple Company { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }


        public ICollection<DotKhamLineDisplay> Lines { get; set; } = new List<DotKhamLineDisplay>();

        //public ICollection<DotKhamStep> Steps { get; set; } = new List<DotKhamStep>();

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
