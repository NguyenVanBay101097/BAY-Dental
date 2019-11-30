using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductPricelist: BaseEntity
    {
        public ProductPricelist()
        {
            Active = true;
            Sequence = 16;
        }

        public string Name { get; set; }

        public bool Active { get; set; }

        public ICollection<ProductPricelistItem> Items { get; set; } = new List<ProductPricelistItem>();

        public int Sequence { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Gán datestart cho tất cả items
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        /// Gán dateend cho tất cả items
        /// </summary>
        public DateTime? DateEnd { get; set; }

        public Guid? PartnerCategId { get; set; }
        public PartnerCategory PartnerCateg { get; set; }

        public Guid? CardTypeId { get; set; }
        public CardType CardType { get; set; }
    }
}
