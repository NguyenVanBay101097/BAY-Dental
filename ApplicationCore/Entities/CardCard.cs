using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CardCard: BaseEntity
    {
        public CardCard()
        {
            State = "draft";
        }
        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Loại thẻ
        /// </summary>
        public Guid TypeId { get; set; }
        public CardType Type { get; set; }

        /// <summary>
        /// draft: Nháp
        /// confirmed: Chờ cấp thẻ
        /// in_use: Đã cấp thẻ
        /// locked: Đã khóa
        /// cancelled: Đã hủy
        /// </summary>
        public string State { get; set; }

        public DateTime? ActivatedDate { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Tổng điểm tích lũy
        /// </summary>
        public decimal? TotalPoint { get; set; }

        public decimal? PointInPeriod { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public Guid? UpgradeTypeId { get; set; }
        public CardType UpgradeType { get; set; }
    }
}
