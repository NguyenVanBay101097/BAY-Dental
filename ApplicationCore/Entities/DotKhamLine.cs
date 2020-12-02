using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    //Tuong tu mrp production
    public class DotKhamLine: BaseEntity
    {
        public Guid DotKhamId { get; set; }
        public DotKham DotKham { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Công đoạn
        /// </summary>
        public string NameStep { get; set; }

        /// <summary>
        /// Dịch vụ
        /// </summary>
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Tham chiếu để lấy danh sách răng
        /// </summary>
        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        /// <summary>
        /// Chi tiết điều trị
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// răng
        /// </summary>
        public ICollection<DotKhamLineToothRel> ToothRels { get; set; } = new List<DotKhamLineToothRel>();
    }
}
