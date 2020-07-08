﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ApplyDiscountSaleOrderViewModel
    {
        public Guid saleOrderId { get; set; }
        /// <summary>
        /// percentage : phần trăm
        /// fixed : tiền mặt
        /// </summary>
        public string DiscountType { get; set; }
        /// <summary>
        /// phần trăm
        /// </summary>
        public decimal? DiscountPercent { get; set; }
        /// <summary>
        /// tiền mặt
        /// </summary>
        public decimal? DiscountFixed { get; set; }
    }
}
