﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineSave
    {
        public Guid Id { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string Name { get; set; }

        public decimal Discount { get; set; }

        public Guid? ProductId { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public Guid? ToothCategoryId { get; set; }

        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();

        public string State { get; set; }

        public string DiscountType { get; set; }

        public decimal? DiscountFixed { get; set; }

        /// <summary>
        /// Số tiền đã thanh toán
        /// </summary>
        public decimal? AmountPaid { get; set; }

        /// <summary>
        /// Tiền còn nợ
        /// </summary>
        public decimal? AmountResidual { get; set; }

        public string SalesmanId { get; set; }
    }
}
