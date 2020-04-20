using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardOrder: BaseEntity
    {
        public ServiceCardOrder()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            BuyType = "one";
        }

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng sẽ ghi công nợ
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Khách hàng thừa hưởng thẻ
        /// </summary>
        public Guid? InheritedPartnerId { get; set; }
        public Partner InheritedPartner { get; set; }

        /// <summary>
        /// Mua 1 hay mua nhiều
        /// one: 1
        /// many: nhiều
        /// </summary>
        public string BuyType { get; set; }

        public Guid CardTypeId { get; set; }
        public ServiceCardType CardType { get; set; }

        /// <summary>
        /// Ngày bán
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Ngày cấp thẻ
        /// </summary>
        public DateTime? ActivatedDate { get; set; }

        /// <summary>
        /// Người bán
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid? MoveId { get; set; }
        public AccountMove Move { get; set; }

        public string State { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<ServiceCardCard> Cards { get; set; } = new List<ServiceCardCard>();

        public ICollection<ServiceCardOrderPartnerRel> PartnerRels { get; set; } = new List<ServiceCardOrderPartnerRel>();
    }
}
