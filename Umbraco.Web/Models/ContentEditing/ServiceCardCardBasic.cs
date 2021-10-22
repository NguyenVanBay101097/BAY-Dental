using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardCardBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }
        public string State { get; set; }
        public Guid CardTypeId { get; set; }
        public ServiceCardTypeSimple CardType { get; set; }
        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }
    }

    public class ServiceCardCardResponse
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ngày bắt đầu
        /// </summary>
        public DateTime? ActivatedDate { get; set; }
        /// <summary>
        /// ngày kết thúc
        /// </summary>
        public DateTime? ExpiredDate { get; set; }

        /// <summary>
        /// draft: Nháp
        /// confirmed: Chờ cấp thẻ
        /// in_use: Đang sử dụng
        /// locked: Đã khóa
        /// cancelled: Đã hủy
        public string State { get; set; }
        public Guid CardTypeId { get; set; }
        public ServiceCardTypeBasic CardType { get; set; }
        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public ProductPricelistItemDisplay ProductPricelistItem { get; set; }
    }

    public class ServiceCardCardFilter
    {
        public Guid? PartnerId { get; set; }

        public Guid? ProductId { get; set; }

        public string State { get; set; }
    }
}
