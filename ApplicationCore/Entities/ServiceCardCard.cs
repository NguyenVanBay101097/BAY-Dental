﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardCard : BaseEntity
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
        /// <summary>
        /// ngày bắt đầu
        /// </summary>
        public DateTime? ActivatedDate { get; set; }
        /// <summary>
        /// ngày kết thúc
        /// </summary>
        public DateTime? ExpiredDate { get; set; }

        /// <summary>
        /// Số tiền trong thẻ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Số tiền còn lại
        /// </summary>
        public decimal? Residual { get; set; }

        /// <summary>
        /// draft: Nháp
        /// in_use: Đang sử dụng
        /// locked: Đã khóa
        /// cancelled: Đã hủy
        /// </summary>
        public string State { get; set; }

        public Guid? SaleLineId { get; set; }
        public ServiceCardOrderLine SaleLine { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string Barcode { get; set; }

        public ICollection<SaleOrderServiceCardCardRel> SaleOrderCardRels { get; set; } = new List<SaleOrderServiceCardCardRel>();
    }
}
