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
        /// Ngày
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Chuẩn đoán, ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Liên kết với đợt khám nào?
        /// </summary>
        public Guid? DotKhamId { get; set; }

        /// <summary>
        /// Tài khoản tạo toa thuốc này
        /// </summary>
        public string UserId { get; set; }

        public Guid CompanyId { get; set; }

        public IEnumerable<ToaThuocLineSave> Lines { get; set; } = new List<ToaThuocLineSave>();
    }
}
