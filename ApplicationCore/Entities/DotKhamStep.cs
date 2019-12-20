﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class DotKhamStep : BaseEntity
    {
        public DotKhamStep()
        {
            State = "confirmed";
            IsInclude = true;
            IsDone = false;
        }

        public DotKhamStep(DotKhamStep step)
        {
            State = "confirmed";
            IsInclude = true;
            IsDone = false;
            Name = step.Name;
            ProductId = step.ProductId;
            DotKhamId = step.DotKhamId;
            SaleLineId = step.SaleLineId;
            SaleOrderId = step.SaleOrderId;
            Order = step.Order;
        }

        public string Name { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? DotKhamId { get; set; }
        public DotKham DotKham { get; set; }

        public Guid? InvoicesId { get; set; }
        public AccountInvoice Invoice { get; set; }

        public Guid? SaleLineId { get; set; }
        public SaleOrderLine SaleLine { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        /// <summary>
        /// Trạng thái: chưa tiến hành, đang tiến hành, hoàn thành
        /// draft: chưa tiến hành
        /// progress: đang tiến hành
        /// done: hoàn thành
        /// </summary>
        public string State { get; set; }

        public bool IsDone { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// Công đoạn này có được sử dụng hay ko
        /// </summary>
        public bool IsInclude { get; set; }
    }
}
