﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Phiếu điều trị nha khoa
    /// </summary>
    public class SaleOrder: BaseEntity
    {
        public SaleOrder()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            Name = "/";
        }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public decimal? AmountTax { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTotal { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string State { get; set; }

        public string Name { get; set; }

        public ICollection<SaleOrderLine> OrderLines { get; set; } = new List<SaleOrderLine>();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Nhân viên, bác sĩ điều trị
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
