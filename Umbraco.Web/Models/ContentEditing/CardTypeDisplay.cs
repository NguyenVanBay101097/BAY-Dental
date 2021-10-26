using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CardTypeDisplay
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public decimal? BasicPoint { get; set; }

        public Guid? PricelistId { get; set; }
        public IEnumerable<ProductPricelistItemDisplay> ProductPricelistItems { get; set; } = new List<ProductPricelistItemDisplay>();
        public decimal Discount { get; set; }

        //Cấu hình thời hạn hết hạn
        public int NbPeriod { get; set; }

        /// <summary>
        /// month: Tháng, year: Năm
        /// </summary>
        public string Period { get; set; }
        public string Color { get; set; }
    }

    public class CardTypeSave
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public decimal? BasicPoint { get; set; }

        public Guid? PricelistId { get; set; }
        public IEnumerable<ProductPricelistItemCreate> ProductPricelistItems { get; set; } = new List<ProductPricelistItemCreate>();
        public decimal Discount { get; set; }

        //Cấu hình thời hạn hết hạn
        public int NbPeriod { get; set; }

        /// <summary>
        /// month: Tháng, year: Năm
        /// </summary>
        public string Period { get; set; }
        public string Color { get; set; }
    }
}
