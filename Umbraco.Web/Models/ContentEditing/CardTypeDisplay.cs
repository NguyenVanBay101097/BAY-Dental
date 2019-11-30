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
        public ProductPricelistBasic Pricelist { get; set; }
        public decimal Discount { get; set; }

        //Cấu hình thời hạn hết hạn
        public int NbPeriod { get; set; }

        /// <summary>
        /// month: Tháng, year: Năm
        /// </summary>
        public string Period { get; set; }
    }
}
