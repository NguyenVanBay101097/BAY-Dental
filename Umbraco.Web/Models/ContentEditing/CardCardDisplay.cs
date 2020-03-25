using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CardCardDisplay
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Loại thẻ
        /// </summary>
        public Guid TypeId { get; set; }
        public CardTypeBasic Type { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        /// <summary>
        /// Tổng điểm tích lũy
        /// </summary>
        public decimal? TotalPoint { get; set; }

        public string State { get; set; }

        public DateTime? ActivatedDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public bool IsExpired { get; set; }

        public Guid? UpgradeTypeId { get; set; }
    }
}
