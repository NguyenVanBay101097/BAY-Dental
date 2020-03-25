using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Loại thẻ thành viên: member, gold, vip
    /// </summary>
    public class CardType: BaseEntity
    {
        /// <summary>
        /// Tên loại card
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Số điểm cơ bản để có thể nâng cấp lên thẻ này
        /// </summary>
        public decimal? BasicPoint { get; set; }

        public decimal Discount { get; set; }

        public Guid? PricelistId { get; set; }
        public ProductPricelist Pricelist { get; set; }

        //Cấu hình thời hạn hết hạn
        public int NbPeriod { get; set; }

        /// <summary>
        /// month: Tháng, year: Năm
        /// </summary>
        public string Period { get; set; }

        public int? Sequence { get; set; }

        public string Note { get; set; }
    }
}
