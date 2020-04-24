using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardCard: BaseEntity
    {
        public ServiceCardCard()
        {
            State = "draft";
        }
        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }

        public Guid CardTypeId { get; set; }
        public ServiceCardType CardType { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public DateTime? ActivatedDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        /// <summary>
        /// Số tiền trong thẻ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Số tiền còn lại
        /// </summary>
        public decimal? Residual { get; set; }

        public string State { get; set; }

        public Guid? OrderId { get; set; }
        public ServiceCardOrder Order { get; set; }

        public ICollection<SaleOrderServiceCardCardRel> SaleOrderCardRels { get; set; } = new List<SaleOrderServiceCardCardRel>();
    }
}
