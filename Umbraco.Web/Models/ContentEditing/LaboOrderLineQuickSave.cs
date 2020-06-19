﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderLineQuickSave
    {
        public LaboOrderLineQuickSave()
        {
            State = "draft";
        }


        public string Name { get; set; }

        public int? Sequence { get; set; }

        public decimal ProductQty { get; set; }

        public Guid? ProductId { get; set; }

        /// <summary>
        /// Màu sắc
        /// </summary>
        public string Color { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal PriceSubtotal { get; set; }

        public decimal PriceTotal { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        public Guid? ToothCategoryId { get; set; }

        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();

        public string State { get; set; }

        public string Note { get; set; }
    }
}
