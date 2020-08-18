using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Tooth: BaseEntity
    {
        /// <summary>
        /// Số răng: 11, 12
        /// </summary>
        public string Name { get; set; }

        public Guid CategoryId { get; set; }
        public ToothCategory Category { get; set; }

        /// <summary>
        /// Vị trí hàm: trên hay dưới
        /// </summary>
        public string ViTriHam { get; set; }

        /// <summary>
        /// Vị trí răng: trái hay phải
        /// </summary>
        public string Position { get; set; }

        public ICollection<SaleOrderLineToothRel> SaleLineToothRels { get; set; } = new List<SaleOrderLineToothRel>();
    }
}
