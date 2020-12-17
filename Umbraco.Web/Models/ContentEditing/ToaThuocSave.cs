using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ToaThuocSave
    {
        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Liên kết với đợt khám nào?
        /// </summary>
        public Guid? DotKhamId { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }

        public IEnumerable<ToaThuocLineSave> Lines { get; set; } = new List<ToaThuocLineSave>();

        /// <summary>
        /// Lời dặn
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ngày Tái Khám
        /// </summary>
        public DateTime Date { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? SaleOrderId { get; set; }

    }

    public class ToaThuocCreateFromUI
    {
        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Liên kết với đợt khám nào?
        /// </summary>
        public Guid? DotKhamId { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }

        public IEnumerable<ToaThuocLineSave> Lines { get; set; } = new List<ToaThuocLineSave>();

        /// <summary>
        /// Lời dặn
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ngày Tái Khám
        /// </summary>
        public DateTime Date { get; set; }

        public bool SaveSamplePrescription { get; set; }

        public string NameSamplePrescription { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? SaleOrderId { get; set; }

    }
}
