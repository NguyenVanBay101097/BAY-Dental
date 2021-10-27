using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CardCardPaged
    {
        public CardCardPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public Guid? PartnerId { get; set; }
        public string Barcode { get; set; }
        public string State { get; set; }
        public bool? IsExpired { get; set; }
    }

    public class GetCardCardFilter
    {
        public Guid? PartnerId { get; set; }

        public Guid? ProductId { get; set; }

        public string State { get; set; }
    }

    public class CardCardResponse
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// draft: Nháp
        /// confirmed: Chờ cấp thẻ
        /// in_use: Đã cấp thẻ
        /// locked: Đã khóa
        /// cancelled: Đã hủy
        /// </summary>
        public string State { get; set; }
        public Guid TypeId { get; set; }
        public CardTypeBasic Type { get; set; }

        public string Barcode { get; set; }

        public ProductPricelistItemDisplay ProductPricelistItem { get; set; }
    }
}
