using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderLineSave
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? Sequence { get; set; }

        public decimal ProductQty { get; set; }

        public Guid? ProductId { get; set; }

        /// <summary>
        /// Màu sắc
        /// </summary>
        public string Color { get; set; }

        public decimal PriceUnit { get; set; }

        public Guid? ToothCategoryId { get; set; }

        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();


        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        public string Note { get; set; }
    }
}
