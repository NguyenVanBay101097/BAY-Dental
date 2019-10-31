using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderSave
    {
        /// <summary>
        /// Vendor Reference
        /// </summary>
        public string PartnerRef { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid PartnerId { get; set; }

        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Ngày nhận (dự kiến)
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        public ICollection<LaboOrderLineSave> OrderLines { get; set; } = new List<LaboOrderLineSave>();

        public Guid? DotKhamId { get; set; }
    }
}
