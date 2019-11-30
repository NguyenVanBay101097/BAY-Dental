using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleSettings: BaseEntity
    {
        /// <summary>
        /// Số tiền mua hàng để được 1 điểm
        /// </summary>
        public decimal? PointExchangeRate { get; set; }
    }
}
