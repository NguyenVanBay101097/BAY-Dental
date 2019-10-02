﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderDisplay
    {
        public SaleOrderDisplay()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            Name = "/";
        }
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public decimal? AmountTax { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTotal { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string State { get; set; }

        public string Name { get; set; }

        public IEnumerable<SaleOrderLineDisplay> OrderLines { get; set; } = new List<SaleOrderLineDisplay>();

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Nhân viên, bác sĩ điều trị
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }
    }
}
