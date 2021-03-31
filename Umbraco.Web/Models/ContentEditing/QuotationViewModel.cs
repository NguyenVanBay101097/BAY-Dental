﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class QuotationBasic
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Mã báo giá
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public PartnerSimple Partner { get; set; }
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Người báo giá
        /// </summary>
        public ApplicationUserSimple User { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// Ngày báo giá
        /// </summary>
        public DateTime DateQuotation { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        public int DateApplies { get; set; }

        /// <summary>
        /// Ngày kết thúc báo giá
        /// </summary>
        public DateTime? DateEndQuotation { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Tổng tiền báo giá
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// trạng thái
        /// </summary>
        public string State { get; set; }
    }

    public class QuotationDisplay
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Mã báo giá
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public PartnerSimple Partner { get; set; }
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Người báo giá
        /// </summary>
        public ApplicationUserSimple User { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// Ngày báo giá
        /// </summary>
        public DateTime DateQuotation { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        public int DateApplies { get; set; }

        /// <summary>
        /// Ngày kết thúc báo giá
        /// </summary>
        public DateTime? DateEndQuotation { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Tổng tiền báo giá
        /// </summary>
        public decimal? TotalAmount { get; set; }

        //public IEnumerable<QuotationLine> MyProperty { get; set; }
    }

    public class QuotationSave
    {

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Người báo giá
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Ngày báo giá
        /// </summary>
        public DateTime DateQuotation { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        public int DateApplies { get; set; }

        /// <summary>
        /// Ngày kết thúc báo giá
        /// </summary>
        public DateTime? DateEndQuotation { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        public IEnumerable<QuotationLineSave> QuotationLines { get; set; } = new List<QuotationLineSave>();
    }

    public class QuotationPaged
    {
        public QuotationPaged()
        {
            Limit = 1;
            Offset = 0;
        }
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public string Search { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public Guid? PartnerId { get; set; }

    }

    public class QuotationSimple
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}


